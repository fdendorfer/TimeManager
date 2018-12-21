// Element Lists, which should not be auto initialized, because they need options
let modalList = document.querySelectorAll('#absenceWindow, #overtimeWindow');
modalList.forEach((elem) => 
{
  elem.classList.add('no-autoinit');
})
let datePickerList = document.querySelectorAll('#absenceWindow #dateFrom, #absenceWindow #dateTo, #overtimeWindow #date')
datePickerList.forEach((elem) => 
{
  elem.classList.add('no-autoinit');
})

// This initializes all Materialize components
document.addEventListener('DOMContentLoaded', function() 
{
  // Build Materialize modals
  materializeBuilder(M.Modal, modalList, {
    onCloseStart: (elem) => {
    elem.querySelector("form").reset();
    M.updateTextFields();
  }});
  // Build Materialize datepickers
  materializeBuilder(M.Datepicker, datePickerList, { container: 'body', format: "dd.mm.yyyy"});

  // Let Materialize search for unitialized components, without class no-autoinit
  M.AutoInit();
});

// Helper function to build multiple components for the same type with the same options
function materializeBuilder(component, elements, options)
{
  elements.forEach(element => {
    component.init(element, options);
  });
}

function absenceFormSubmit() 
{

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