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
document.addEventListener("DOMContentLoaded", function(e) {
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
  var buildDatepicker = materializeBuilder(M.Datepicker, datePickerList, {
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
  var buildTimepicker = materializeBuilder(M.Timepicker, timePickerList, {
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

// This prevents the form from submitting normally, because it would reload the whole page, even when there were errors on the page. Like this only the error fields get switched
document.addEventListener("DOMContentLoaded", function(e) {
  $('#absenceForm, #overtimeForm').on('submit', function(e) {
    e.preventDefault();
    $.ajax({
      url: $(this).attr('action') || window.location.pathname,
      type: 'POST',
      data: $(e.target).serialize(),
      success: function(data, textStatus, jqXHR) {
        if(jqXHR.status == 202)
          location.reload();

        let parser = new DOMParser();
        let errorSpansOld = document.querySelectorAll('.red-text');
        let errorSpansNew = parser.parseFromString(data, "text/html").querySelectorAll('.red-text');
        for(var i = 0; i < errorSpansNew.length; i++) {
          errorSpansOld[i].outerHTML = errorSpansNew[i].outerHTML;
        }
      },
      error: function(jXHR, textStatus, errorThrown) {
        alert(errorThrown);
        location.reload();
      }
    });
  });
});
