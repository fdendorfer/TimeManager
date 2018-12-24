// Element Lists, which should not be auto initialized, because they need options
let modalList = document.querySelectorAll("#absenceWindow, #overtimeWindow");
modalList.forEach(elem => {
  elem.classList.add("no-autoinit");
});
let datePickerList = document.querySelectorAll(
  "#absenceWindow #dateFrom, #absenceWindow #dateTo, #overtimeWindow #date"
);
datePickerList.forEach(elem => {
  elem.classList.add("no-autoinit");
});
let timePickerList = document.querySelectorAll(
  "#absenceWindow #timeFrom, #absenceWindow #timeTo"
);
timePickerList.forEach(elem => {
  elem.classList.add("no-autoinit");
});

// This initializes all Materialize components
document.addEventListener('DOMContentLoaded', function(e) {
  // Build Materialize modals
  var buildModal = materializeBuilder(M.Modal, modalList, {
    onCloseStart: elem => {
      elem.querySelector("form").reset();
      M.updateTextFields();
      document.querySelectorAll(".field-validation-error").forEach(elem => {
        elem.innerHTML = "";
      });
    }
  });
  // Build Materialize datepickers
  materializeBuilder(M.Datepicker, datePickerList, {
    format: "dd.mm.yyyy",
    firstDay: 1,
    showDaysInNextAndPreviousMonths: true,
    container: "body",
    i18n: {
      cancel: "Abbrechen",
      done: "Ok",
      months: [
        "Januar",
        "Februar",
        "März",
        "April",
        "Mai",
        "Juni",
        "Juli",
        "August",
        "September",
        "Oktober",
        "November",
        "Dezember"
      ],
      monthsShort: [
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "Mai",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Okt",
        "Nov",
        "Dez"
      ],
      weekdays: [
        "Sonntag",
        "Montag",
        "Dienstag",
        "Mittwoch",
        "Donnerstag",
        "Freitag",
        "Samstag"
      ],
      weekdaysShort: ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"],
      weekdaysAbbrev: ["S", "M", "D", "M", "D", "F", "S"]
    }
  });
  // Build Materialize timepickers
  materializeBuilder(M.Timepicker, timePickerList, {
    container: "body",
    i18n: {
      cancel: "Abbrechen",
      done: "Ok"
    },
    twelveHour: false
  });

  // Let Materialize search for unitialized components, without class no-autoinit
  M.AutoInit();
});

// Helper function to build multiple components for the same type with the same options
function materializeBuilder(component, elements, options) {
  elements.forEach(element => {
    component.init(element, options);
  });
}

/* 
$(document).ready(function(){
  // OwnTimes
  $('#absenceWindow').modal({
    onCloseStart: (modal) => 
    {
      $(modal).modal('destroy');
      $(modal).modal();
    }
  });
  $('#absenceWindow #dateFrom').datepicker({format: 'dd.mm.yyyy'});
  $('#absenceWindow #dateTo').datepicker({format: 'dd.mm.yyyy'});
  $('#overtimeWindow').modal();
  $('#overtimeWindow #date').datepicker({format: 'dd.mm.yyyy'});
  $('#overtimeWindow select').formSelect();
}) */

$(document).ready(function() {
  $("#absenceForm").on("submit", function(e) {
    e.preventDefault();
    $.ajax({
      url: $(this).attr("action") || window.location.pathname,
      type: "POST",
      data: $("#absenceForm").serialize(),
      success: function(data) {
        $("#absenceForm").replaceWith($(data).find("#absenceForm"));
      },
      error: function(jXHR, textStatus, errorThrown) {
        alert(errorThrown);
      }
    });
  });
});
