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
