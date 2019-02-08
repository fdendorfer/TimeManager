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

// * Controlling
// Checkbox click listener to filter table
$('#Controlling #filter').click(() => {
  let uncheckedOnly = $('#Controlling #filter').prop('checked');
  writeControllingTable(uncheckedOnly);
});

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
