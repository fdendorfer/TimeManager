
//Initialize all materialize components
$(document).ready(function(){
  // OwnTimes
  $('#absenceWindow').modal();
  $('#absenceWindow #absenceFrom').datepicker({format: 'dd.mm.yyyy'});
  $('#absenceWindow #absenceTo').datepicker({format: 'dd.mm.yyyy'});
  $('#overtimeWindow').modal();
  $('#overtimeWindow #date').datepicker({format: 'dd.mm.yyyy'});
  $('#overtimeWindow select').formSelect();
})