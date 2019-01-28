$(document).ready(() => {
  materializeStuff();
});

function materializeStuff() {
  // Element Lists, which should not be auto initialized, because they need options
  let modalList = document.querySelectorAll('#absenceWindow, #overtimeWindow, #userWindow');
  $(modalList).addClass('no-autoinit');
  let datePickerList = document.querySelectorAll('#absenceWindow #dateFrom, #absenceWindow #dateTo, #overtimeWindow #date');
  $(datePickerList).addClass('no-autoinit');
  let timePickerList = document.querySelectorAll('#absenceWindow #timeFrom, #absenceWindow #timeTo');
  $(timePickerList).addClass('no-autoinit');

  // Build Materialize modals
  materializeBuilder(M.Modal, modalList, {
    // Clear text fields and error messages, when modal gets closed
    onCloseStart: elem => {
      elem.querySelector('form').reset();
      document.querySelectorAll('.field-validation-error').forEach(elem => {
        elem.innerHTML = '';
      });
    }
  });
  // Build Materialize datepickers
  materializeBuilder(M.Datepicker, datePickerList, {
    // Displayformat
    format: 'dd.mm.yyyy',
    // Start der Woche, 0 = Sunday
    firstDay: 1,
    // Fills the already displayed weeks with days from previous and next months
    showDaysInNextAndPreviousMonths: true,
    // Parent element
    container: 'body',
    // Language options, i18n = internationalization
    i18n: {
      cancel: 'Abbrechen',
      done: 'Ok',
      months: [
        'Januar',
        'Februar',
        'März',
        'April',
        'Mai',
        'Juni',
        'Juli',
        'August',
        'September',
        'Oktober',
        'November',
        'Dezember'
      ],
      monthsShort: [
        'Jan',
        'Feb',
        'Mar',
        'Apr',
        'Mai',
        'Jun',
        'Jul',
        'Aug',
        'Sep',
        'Okt',
        'Nov',
        'Dez'
      ],
      weekdays: [
        'Sonntag',
        'Montag',
        'Dienstag',
        'Mittwoch',
        'Donnerstag',
        'Freitag',
        'Samstag'
      ],
      weekdaysShort: ['So', 'Mo', 'Di', 'Mi', 'Do', 'Fr', 'Sa'],
      weekdaysAbbrev: ['S', 'M', 'D', 'M', 'D', 'F', 'S']
    }
  });
  // Build Materialize timepickers
  materializeBuilder(M.Timepicker, timePickerList, {
    container: 'body',
    i18n: {
      cancel: 'Abbrechen',
      done: 'Ok'
    },
    twelveHour: false
  });

  // Let Materialize search for unitialized components, without class no-autoinit
  M.AutoInit();
}

// Helper function to build multiple components for the same type with the same options
function materializeBuilder(component, elements, options) {
  elements.forEach(element => {
    component.init(element, options);
  });
}

// This prevents the forms from submitting normally, because it would reload the whole page, even when there were errors on the page
$('#absenceForm, #overtimeForm, #userForm').on('submit', e => {
  e.preventDefault();
  $.ajax({
    url: $(e.target).attr('action'), // This gets the full called url with the handler
    type: 'POST',
    data: $(e.target).serialize(),
    success: (data, textStatus, jqXHR) => {
      // If ModelState is valid then a the specified HTTP state 202 is returned
      if (jqXHR.status === 202)
        location.reload();

      let errorSpansOld = $('.red-text');
      let errorSpansNew = $(data).find('.red-text');
      // Replace all the old error spans with the new content
      for (var i = 0; i < errorSpansNew.length; i++) {
        errorSpansOld[i].outerHTML = errorSpansNew[i].outerHTML;
      }
    },
    error: () => {
      alert('Fehler bei der Verarbeitung des Formulars. Überprüfen sie, ob ihre Aktion durchgeführt wurde und versuchen Sie es allenfalls erneut.');
      location.reload();
    }
  });
});

// * OwnTimes
// Checkbox and Select listener to filter tables
$('#OwnTimes #filter, #OwnTimes select').change(function (e) {
  let last30days = $('#OwnTimes #filter').prop('checked');
  let selectedUser = $('#OwnTimes select').val();
  writeOwnTimesTables(last30days, selectedUser);
});

function writeOwnTimesTables(last30days, selectedUser) {
  // Remove old rows
  $('#OwnTimes tbody tr').remove();

  // Call OnGet() to get json with all data filtered
  $.ajax({
    url: `/OwnTimes?last30days=${last30days}&selectedUser=${selectedUser}`,
    success: (data) => {
      // Table selectors
      let absenceTable = document.querySelectorAll('#OwnTimes tbody')[0];
      let overtimeTable = document.querySelectorAll('#OwnTimes tbody')[1];
      // Fill first table with data from json
      data[0].forEach(e => {
        // New row
        let row = absenceTable.insertRow(absenceTable.rows.length);
        let i = 0;
        // Inserting values from json into table
        row.insertCell(i).appendChild(document.createTextNode(new Date(e.absentFrom).toLocaleString('de-CH')));
        row.insertCell(++i).appendChild(document.createTextNode(new Date(e.absentTo).toLocaleString('de-CH')));
        row.insertCell(++i).appendChild(document.createTextNode(e.reason));
        // Checkbox
        let checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" /><span></span></label>`);
        if (e.approved)
          checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" checked /><span></span></label>`);
        row.insertCell(++i).appendChild(checkbox[0]);
        // Edit and delete buttons
        let buttons = $(`<div><button type="button" class="btn-small" onclick="absenceEdit ('${e.id}')">
<i class="material-icons">edit</i>
</button>
<button type="button" class="btn-small" onclick="absenceDelete('${e.id}')">
<i class="material-icons">delete</i>
</button></div>`);
        row.insertCell(++i).appendChild(buttons[0]);
      });

      // Fill second table with data from json
      data[1].forEach(e => {
        // New row
        let row = overtimeTable.insertRow(overtimeTable.rows.length);
        let i = 0;
        // Inserting values from json into table
        row.insertCell(i).appendChild(document.createTextNode(new Date(e.o.date).toLocaleDateString('de-CH')));
        row.insertCell(++i).appendChild(document.createTextNode(e.o.customer));
        row.insertCell(++i).appendChild(document.createTextNode(e.o.hours));
        row.insertCell(++i).appendChild(document.createTextNode(e.od.rate));
        // Edit and delete buttons
        let buttons = $(`<div><button type="button" class="btn-small" onclick="overtimeEdit ('${e.id}')">
<i class="material-icons">edit</i>
</button>
<button type="button" class="btn-small" onclick="overtimeDelete('${e.id}')">
<i class="material-icons">delete</i>
</button></div>`);
        row.insertCell(++i).appendChild(buttons[0]);
      });
    }
  });
}

// Fills absencePartial, when opened for editing
function absenceEdit(id) {
  $.ajax({
    url: '/OwnTimes?handler=Absence&id=' + id,
    success: data => {
      // Array of all input fields in absenceWindow
      let inputs = $('#absenceWindow input');
      // Js object from JSON
      let json = $.parseJSON(data);
      // Prefill fields with data from json
      $(inputs[0]).val(json.AbsenceDateFrom);
      $(inputs[1]).val(json.AbsenceDateTo);
      $(inputs[2]).prop('checked', json.FullDay);
      $(inputs[3]).val(json.AbsenceTimeFrom);
      $(inputs[4]).val(json.AbsenceTimeTo);
      $(inputs[5]).prop('checked', json.Negative);

      // Radio buttons Reason
      let reason = $(inputs).filter(`[value='${json.OtherReason}']`);
      if (reason.length > 0) {
        $(reason[0]).prop('checked', true);
      } else {
        $(inputs[11]).prop('checked', true);
        $(inputs[12]).val(json.OtherReason);
      }
      $(inputs[13]).prop('checked', json.Approved);
      $(inputs[14]).val(json.ID);

      $('#absenceWindow').modal('open');
    }
  });
}

function overtimeEdit(id) {
  $('#overtimeWindow').modal('open');
  $.ajax({
    url: '/OwnTimes?handler=Overtime&id=' + id,
    success: data => {
      let inputs = $('#overtimeWindow input');
      let json = $.parseJSON(data);
      $(inputs[0]).val(json.Date);
      $(inputs[1]).val(json.Hours);
      $(inputs[2]).val(json.Customer);

      $('#overtimeWindow select').find(`option[value="${json.IdOvertimeDetail.toUpperCase()}"]`).prop('selected', true);
      $('#overtimeWindow select').formSelect();
      $(inputs[4]).val(json.ID);

    }
  });
}

function absenceDelete(id) {
  $.ajax({
    url: '/OwnTimes?handler=Absence&id=' + id,
    method: 'DELETE',
    beforeSend: xhr => {
      xhr.setRequestHeader('XSRF-TOKEN', $('input:hidden[name="__RequestVerificationToken"]').val());
    },
    success: () => {
      location.reload();
    }
  });
}

function overtimeDelete(id) {
  $.ajax({
    url: '/OwnTimes?handler=Overtime&id=' + id,
    method: 'DELETE',
    beforeSend: xhr => {
      xhr.setRequestHeader('XSRF-TOKEN', $('input:hidden[name="__RequestVerificationToken"]').val());
    },
    success: () => {
      location.reload();
    }
  });
}


// * Controlling
// Checkbox click listener to filter table
$('#Controlling #filter').click(() => {
  let uncheckedOnly = $('#Controlling #filter').prop('checked');
  writeControllingTable(uncheckedOnly);
});

function writeControllingTable(uncheckedOnly) {
  // Remove old rows
  $('#Controlling tbody tr').remove();

  // Call OnGet() to get json with all users filtered
  $.ajax({
    url: '/Controlling?uncheckedOnly=' + uncheckedOnly,
    success: (data) => {
      // Table selector
      let table = document.querySelector('#Controlling tbody');
      // For each line in json
      data.forEach(e => {
        // New row
        let row = table.insertRow(table.rows.length);
        let i = 0;
        // Inserting values from json into table
        row.insertCell(i).appendChild(document.createTextNode(e.name));
        row.insertCell(++i).appendChild(document.createTextNode(new Date(e.absentFrom).toLocaleString('de-CH')));
        row.insertCell(++i).appendChild(document.createTextNode(new Date(e.absentTo).toLocaleString('de-CH')));
        row.insertCell(++i).appendChild(document.createTextNode(e.reason));
        let checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" /><span></span></label>`);
        if (e.approved)
          checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" checked /><span></span></label>`);
        row.insertCell(++i).appendChild(checkbox[0]);
      });
    }
  });
}

$('#Controlling #ferienliste').click(() => {
  window.open('/Controlling?handler=Ferienliste', '_blank');
});

$('#Controlling #überzeitkontrolle').click(() => {
  window.open('/Controlling?handler=Überzeitkontrolle', '_blank');
});

// An advanced user can approve the absences in /Controlling with the checkboxes
$('#Controlling').on('change', 'input[name="isIO"]', e => {
  let idAbsence = $(e.target).attr('data-id-absence');
  $.ajax({
    url: `/Controlling?idAbsence=${idAbsence}&value=${e.currentTarget.checked}`,
    type: 'POST',
    beforeSend: xhr => {
      xhr.setRequestHeader('XSRF-TOKEN', $('input:hidden[name="__RequestVerificationToken"]').val());
    },
    success: () => {
      location.reload();
    }
  });
});


// * Users
// Checkbox click listener to filter table
$('#Users #filter').click(() => {
  let activeFilter = $('#Users #filter').prop('checked');
  writeUsersTable(activeFilter);
});

function writeUsersTable(activeFilter) {
  // Remove old rows
  $('#Users tbody tr').remove();

  // Call OnGet() to get json with all users filtered
  $.ajax({
    url: '/Users?onlyActive=' + activeFilter,
    success: (data) => {
      // Table selector
      let table = document.querySelector('#Users tbody');
      // For each line in json
      data.forEach(e => {
        // New row
        let row = table.insertRow(table.rows.length);
        let i = 0;
        // Inserting values from json into table
        row.insertCell(i).appendChild(document.createTextNode(e.username));
        row.insertCell(++i).appendChild(document.createTextNode(e.firstname));
        row.insertCell(++i).appendChild(document.createTextNode(e.lastname));
        row.insertCell(++i).appendChild(document.createTextNode(e.department));
        // Edit and delete buttons
        let buttons = $(`<div><button type="button" class="btn-small" onclick="userEdit ('${e.id}')">
<i class="material-icons">edit</i>
</button>
<button type="button" class="btn-small" onclick="userDelete('${e.id}')">
<i class="material-icons">delete</i>
</button></div>`);
        row.insertCell(++i).appendChild(buttons[0]);
      });
    },
    error: () => {
      alert('An error has occured');
    }
  });
}

function userEdit(id) {
  $('#userWindow').modal('open');
  $.ajax({
    url: '/Users?id=' + id,
    success: data => {
      let inputs = $('#userWindow input');
      let json = $.parseJSON(data);
      $(inputs[0]).val(json.Username);
      $(inputs[2]).val(json.Firstname);
      $(inputs[3]).val(json.Lastname);
      $('#userWindow select:eq(0)').find(`option[value="${json.Department}"]`).prop('selected', true);
      $('#userWindow select:eq(0)').formSelect();
      $(inputs[5]).val(json.Holidays);
      $(inputs[6]).prop('checked', json.Deactivated);

      $('#userWindow select:eq(1)').find(`option[value="${json.IdPermission.toUpperCase()}"]`).prop('selected', true);
      $('#userWindow select:eq(1)').formSelect();
      $(inputs[8]).val(json.ID);
    }
  });
}

function userDelete(id) {
  $.ajax({
    url: '/Users?id=' + id,
    method: 'DELETE',
    beforeSend: xhr => {
      xhr.setRequestHeader('XSRF-TOKEN', $('input:hidden[name="__RequestVerificationToken"]').val());
    },
    success: () => {
      location.reload();
    }
  });
}