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
    },
    // Sets 
    onSelect: elem => {
      switch (elem.getUTCDay()) { // GetUTCDay according to i18n.weekdays option
        case 6: // Sunday
          $('#overtimeForm select').val($('#overtimeForm option')[2].value)
          break;
        case 5: // Saturday
          $('#overtimeForm select').val($('#overtimeForm option')[1].value)
          break;
        default: // Unter the week
          $('#overtimeForm select').val($('#overtimeForm option')[0].value)
          break;
      }
      $('#overtimeForm select').formSelect();
    }
  });
  // Build Materialize timepickers
  //materializeBuilder(M.Timepicker, timePickerList, {
  //  container: 'body',
  //  i18n: {
  //    cancel: 'Abbrechen',
  //    done: 'Ok'
  //  },
  //  twelveHour: false
  //});

  // Let Materialize search for unitialized components, without class no-autoinit
  M.AutoInit();
}

// Helper function to build multiple components for the same type with the same options
function materializeBuilder(component, elements, options) {
  elements.forEach(element => {
    component.init(element, options);
  });
}

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
        let checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" disabled /><span></span></label>`);
        if (e.approved)
          checkbox = $(`<label><input name="isIO" data-id-absence="${e.idAbsence}" type="checkbox" class="filled-in" checked disabled /><span></span></label>`);
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
        let buttons = $(`<div><button type="button" class="btn-small" onclick="overtimeEdit ('${e.o.id}')">
<i class="material-icons">edit</i>
</button>
<button type="button" class="btn-small" onclick="overtimeDelete('${e.o.id}')">
<i class="material-icons">delete</i>
</button></div>`);
        row.insertCell(++i).appendChild(buttons[0]);
      });

      // Fill remaining absences and done overtimes 
      $('#absencesRemaining').html(Math.round((data[2][1] - data[2][0] + (data[3] / 8.5)) * 100) / 100);
      $('#doneOvertimes').html(data[3]);
    }
  });
}

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

function absenceEdit(id) {
  $.ajax({
    url: '/OwnTimes?handler=Absence&id=' + id,
    success: data => {
      // Js object from JSON
      let json = $.parseJSON(data);
      // Prefill fields with data from json
      $('#absenceWindow input[name="Absence.AbsenceDateFrom"]').val(json.AbsenceDateFrom);
      $('#absenceWindow input[name="Absence.FromAfternoon"]:eq(0)').prop('checked', !json.FromAfternoon);
      $('#absenceWindow input[name="Absence.FromAfternoon"]:eq(1)').prop('checked', json.FromAfternoon);
      $('#absenceWindow input[name="Absence.AbsenceDateTo"]').val(json.AbsenceDateTo);      
      $('#absenceWindow input[name="Absence.ToAfternoon"]:eq(0)').prop('checked', !json.ToAfternoon);
      $('#absenceWindow input[name="Absence.ToAfternoon"]:eq(1)').prop('checked', json.ToAfternoon);
      $('#absenceWindow input[name="Absence.Negative"]').prop('checked', json.Negative);

      // Radio buttons Reason
      let reason = $('#absenceWindow input[name="Absence.Reason"]').filter(`[value='${json.OtherReason}']`);
      if (reason.length > 0) {
        $(reason[0]).prop('checked', true);
      } else {
        $('#absenceWindow input[name="Absence.Reason"]').last().prop('checked', true);
        $('#absenceWindow input[name="Absence.OtherReason"]').val(json.OtherReason);
      }
      $('#absenceWindow input[name="Absence.Approved"]').prop('checked', json.Approved);
      $('#absenceWindow input[name="Absence.ID"]').val(json.ID);

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
      $(inputs[4]).val(json.Approved);

      $('#overtimeWindow select').find(`option[value="${json.IdOvertimeDetail.toUpperCase()}"]`).prop('selected', true);
      $('#overtimeWindow select').formSelect();
      $(inputs[5]).val(json.ID);
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