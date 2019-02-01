using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using TimeManager.Database;
using TimeManager.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace TimeManager.Extensions
{
  public class ExcelHandler
  {
    public static async Task CreateFerienliste(string department, DatabaseContext db)
    {
      await Task.Run(() =>
      {
        // Path where file will be stored
        var templatePath = @"C:\temp\Ferienliste_template.xlsx";
        using (var outFile = new FileStream(templatePath.Replace("template", department), FileMode.Create, FileAccess.ReadWrite))
        {
          // Workbook
          var wb = new XSSFWorkbook();

          // All years which are used by Absences
          var yearsWithHolidays = db.Absence
            .Join(db.User, a => a.IdUser, u => u.ID, (a, u) => new { a, u })
            .Where((au) => au.u.Department == department)
            .Select(au => new[] { au.a.AbsentFrom.Year, au.a.AbsentTo.Year })
            .SelectMany(m => m)
            .OrderBy(o => o)
            .Distinct()
            .ToArray();

          // Year loop
          foreach (var year in yearsWithHolidays)
          {
            // All users which are used by Absences in this year
            var users = from u in db.User
                        join a in db.Absence on u.ID equals a.IdUser
                        where (year != DateTime.Now.Year) ? (a.AbsentFrom.Year == year || a.AbsentTo.Year == year) : (u.Deactivated == false) // Show all activated users only in current year
                        where (u.Department == department) // Always filter by department
                        orderby u.Username
                        select u;
            List<UserModel> usersThisYear = users.Distinct().ToList();

            // Gets the last week of the year and makes it the total of weeks
            var totalWeeks = GetWeek(DateTime.Parse(year.ToString() + "-12-31"));

            var sh = (XSSFSheet)wb.CreateSheet(year.ToString());

            var colCount = 1 + usersThisYear.Count;
            var rowNum = 0;
            IRow row;
            int colNum = 0;

            // Row 0
            row = sh.CreateRow(rowNum);

            foreach (var user in usersThisYear)
            {
              row.CreateCell(++colNum).SetCellValue(user.Username);
            }

            // Row 1
            row = sh.CreateRow(++rowNum);
            colNum = 0;

            sh.AddMergedRegion(new CellRangeAddress(rowNum, rowNum + 3, colNum, colNum));
            row.CreateCell(colNum).SetCellValue(year);
            foreach (var user in usersThisYear)
            {
              var colLetter = CellReference.ConvertNumToColString(++colNum);
              string formula = $"{colLetter}5/{colLetter}3";
              XSSFCell cell = (XSSFCell)row.CreateCell(colNum);
              cell.SetCellFormula(formula);
              XSSFCellStyle style = (XSSFCellStyle)wb.CreateCellStyle();
              style.SetDataFormat(9);
              cell.CellStyle = style;
            }

            // Row 2
            row = sh.CreateRow(++rowNum);
            colNum = 0;
            foreach (var user in usersThisYear)
            {
              row.CreateCell(++colNum).SetCellValue((double)user.Holidays);
            }

            // Row 3
            row = sh.CreateRow(++rowNum);
            colNum = 0;
            foreach (var user in usersThisYear)
            {
              var colLetter = CellReference.ConvertNumToColString(++colNum);
              string formula = $"{colLetter}3-{colLetter}5";
              row.CreateCell(colNum).SetCellFormula(formula);
            }

            // Row 4
            row = sh.CreateRow(++rowNum);
            colNum = 0;
            foreach (var user in usersThisYear)
            {
              var colLetter = CellReference.ConvertNumToColString(++colNum);
              string formula = $"SUM({colLetter}6:{colLetter}{totalWeeks + 5})";
              row.CreateCell(colNum).SetCellFormula(formula);
            }

            // Calendar week rows
            // columnPerUsers stores all users of current year and adds a string with a number of each holiday for a person, where the poition in the string is the week
            var columnPerUser = new Dictionary<string, string>();

            foreach (var user in usersThisYear)
            {
              columnPerUser.Add(user.Username, new string('0', totalWeeks));

              var absences = db.Absence.Where(a => (a.IdUser == user.ID) && (a.AbsentFrom.Year == year) && (user.Deactivated == false)).ToList();
              var daysInWeeks = new int[totalWeeks];

              foreach (var a in absences)
              {
                // https://stackoverflow.com/questions/13440595/get-list-of-dates-from-startdate-to-enddate
                //Counts days between a.AbsentFrom and a.AbsentTo
                IEnumerable<double> daysToAdd = Enumerable.Range(0, (a.AbsentTo - a.AbsentFrom).Days + 1).ToList().ConvertAll(d => (double)d);
                IEnumerable<DateTime> ListOfDates = daysToAdd.Select(a.AbsentFrom.AddDays).ToList();

                foreach (var date in ListOfDates)
                {
                  if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                  {
                    if (a.Negative)
                    {
                      daysInWeeks[GetWeek(date) - 1] -= 1; // GetWeek doesn't count from 0
                    } else
                    {
                      daysInWeeks[GetWeek(date) - 1] += 1; // GetWeek doesn't count from 0
                    }
                  }
                }
              }
              columnPerUser[user.Username] = string.Join("", daysInWeeks);
            }

            // Writes all holidays from all weeks into rows
            for (var i = 0; i < totalWeeks; i++)
            {
              row = sh.CreateRow(++rowNum);
              colNum = 0;
              row.CreateCell(colNum).SetCellValue("KW" + (i + 1).ToString("D2")); //ToString("D2") for 2 digits like 01
              foreach (var user in columnPerUser)
              {
                row.CreateCell(++colNum).SetCellValue(Int32.Parse(user.Value[i].ToString()));
              }
            }

            // Style
            for (int i = 1; i <= sh.GetRow(0).LastCellNum; i++)
              sh.AutoSizeColumn(i, true); // Not working

            // Conditional formatting
            IConditionalFormattingRule rule;
            IPatternFormatting pf;

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotEqual, "123456");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Black.Index;
            rule.GetFontFormatting().SetFontStyle(false, true);
            rule.CreateBorderFormatting().BorderBottom = rule.CreateBorderFormatting().BorderRight = BorderStyle.Thin;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"A1:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}1") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.LessThan, "0");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Red.Index;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"B4:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}4") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.GreaterThan, "0");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Black.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Grey25Percent.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"B6:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}{sh.LastRowNum + 1}") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.GreaterThan, "1");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.White.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Violet.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"B2:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}2") },
              rule
            );

            rule = (XSSFConditionalFormattingRule)sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.GreaterThan, "0.8");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Green.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.LightGreen.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"B2:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}2") },
              rule
            );

            rule = (XSSFConditionalFormattingRule)sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.LessThanOrEqual, "0.8");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Black.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Red.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"B2:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}2") },
              rule
            );

            rule = (XSSFConditionalFormattingRule)sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotEqual, "123456");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.Violet.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Black.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new CellRangeAddress[] { CellRangeAddress.ValueOf($"B3:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}3") },
              rule
            );

            rule = (XSSFConditionalFormattingRule)sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotEqual, "123456");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.BrightGreen.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Black.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new CellRangeAddress[] { CellRangeAddress.ValueOf($"B4:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}4") },
              rule
            );

            rule = (XSSFConditionalFormattingRule)sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotEqual, "123456");
            rule.CreateFontFormatting().FontColorIndex = IndexedColors.White.Index;
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Black.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new CellRangeAddress[] { CellRangeAddress.ValueOf($"A2:{CellReference.ConvertNumToColString(sh.GetRow(0).LastCellNum - 1)}5"), CellRangeAddress.ValueOf($"A2:A{sh.LastRowNum + 1}") },
              rule
            );
          }

          wb.Write(outFile);
          outFile.Close();
        }
      });
    }

    public static async Task CreateÜberzeitkontrolle(string department, DatabaseContext db)
    {
      await Task.Run(() =>
      {
        var templatePath = @"C:\temp\Überzeitkontrolle_template.xlsx";
        using (var outFile = new FileStream(templatePath.Replace("template", department), FileMode.Create, FileAccess.ReadWrite))
        {
          // Workbook
          var wb = new XSSFWorkbook();

          // Style

          // All users which are used by Overtimes in this year
          var year = DateTime.Now.Year;
          var users = (from u in db.User
                       join o in db.Overtime on u.ID equals o.IdUser
                       where (o.Date.Year == year) && (u.Department == department) && (u.Deactivated == false)
                       orderby u.Username
                       select u).Distinct();

          // Create Overview sheet and add static text
          var sh = wb.CreateSheet("Übersicht");
          var rowNum = 0;
          var row = sh.CreateRow(rowNum);
          var colNum = 0;
          row.CreateCell(colNum).SetCellValue("Übersicht");
          row = sh.CreateRow(++rowNum);
          row.CreateCell(colNum).SetCellValue("Kürzel");
          row.CreateCell(++colNum).SetCellValue("Stunden");
          row.CreateCell(++colNum).SetCellValue("Tage");
          var region = CellRangeAddress.ValueOf("A2:C2");
          var border = (int)BorderStyle.Medium;
          RegionUtil.SetBorderTop(border, region, sh, wb);
          RegionUtil.SetBorderRight(border, region, sh, wb);
          RegionUtil.SetBorderBottom(border, region, sh, wb);
          RegionUtil.SetBorderLeft(border, region, sh, wb);

          // Users loop
          foreach (var user in users)
          {
            // Create sheet per user
            sh = wb.CreateSheet(user.Username);

            // Row 0
            rowNum = 0;
            row = sh.CreateRow(rowNum);
            colNum = 0;
            row.CreateCell(colNum).SetCellValue(year);
            row.CreateCell(++colNum).SetCellValue(user.Username);

            // Row 1
            row = sh.CreateRow(++rowNum);
            colNum = 0;
            row.CreateCell(colNum).SetCellValue("Datum");
            row.CreateCell(++colNum).SetCellValue("Kunde");
            row.CreateCell(++colNum).SetCellValue("OK");
            row.CreateCell(++colNum).SetCellValue("Zeit");
            row.CreateCell(++colNum).SetCellValue("Zuschlag");
            row.CreateCell(++colNum).SetCellValue("Total");
            // Borders
            region = CellRangeAddress.ValueOf("A2:F2");
            border = (int)BorderStyle.Medium;
            RegionUtil.SetBorderTop(border, region, sh, wb);
            RegionUtil.SetBorderRight(border, region, sh, wb);
            RegionUtil.SetBorderBottom(border, region, sh, wb);
            RegionUtil.SetBorderLeft(border, region, sh, wb);

            // Row 2+
            var overtimes = (from o in db.Overtime
                             where (o.Date.Year == year) && (o.IdUser == user.ID)
                             orderby o.Date
                             select o).ToList();
            foreach (var overtime in overtimes)
            {
              row = sh.CreateRow(++rowNum);
              colNum = 0;
              row.CreateCell(colNum).SetCellValue(overtime.Date.ToShortDateString());
              row.CreateCell(++colNum).SetCellValue(overtime.Customer);
              row.CreateCell(++colNum).SetCellValue("");
              row.CreateCell(++colNum).SetCellValue(overtime.Hours.ToString());
              var rate = db.OvertimeDetail.Where(od => od.ID == overtime.IdOvertimeDetail).Select(s => s.Rate);
              row.CreateCell(++colNum).SetCellValue(Convert.ToDouble(rate.Single()));
              row.CreateCell(++colNum).SetCellFormula($"IF(C{rowNum + 1}=\"ok\",D{rowNum + 1}*E{rowNum + 1},0)");
            }

            // Last row
            rowNum = 49; // Is row 50 in excel
            row = sh.CreateRow(rowNum);
            colNum = 1;
            row.CreateCell(colNum).SetCellValue("TOTAL");
            row.CreateCell(colNum += 2).SetCellFormula("F50/8.5");
            row.CreateCell(++colNum).SetCellValue("<<<<");
            row.CreateCell(++colNum).SetCellFormula("SUM(F3:F49)");
            var style = (XSSFCellStyle)wb.CreateCellStyle();
            var font = wb.CreateFont();
            font.FontHeight = wb.GetFontAt(0).FontHeight;
            font.IsBold = true;
            style.SetFont(font);
            foreach (var cell in row)
              cell.CellStyle = style;


            // Styles for entry rows
            var cs = (XSSFCellStyle)wb.CreateCellStyle();
            cs.BorderTop = cs.BorderRight = cs.BorderBottom = cs.BorderLeft = BorderStyle.Thin;
            border = (int)BorderStyle.Thin;
            for (var i = 2; i <= 48; i++)
            {
              var r = sh.GetRow(i);
              if (r != null)
                foreach (var cell in r.Cells)
                  cell.CellStyle = cs;
            }

            // Conditional formatting
            IConditionalFormattingRule rule;
            IPatternFormatting pf;

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotEqual, "123456");
            pf = (XSSFPatternFormatting)rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.LightTurquoise.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf("A50:F50"), CellRangeAddress.ValueOf("F3:F50") },
              rule
            );

            // Add user to overview sheet
            sh = wb.GetSheet("Übersicht");
            row = sh.CreateRow(sh.LastRowNum + 1);
            colNum = 0;
            row.CreateCell(colNum).SetCellValue(user.Username);
            row.CreateCell(++colNum).SetCellFormula(user.Username + "!$F$50");
            row.CreateCell(++colNum).SetCellFormula(user.Username + "!$D$50");
            // Borders for these cells
            style = null;
            style = (XSSFCellStyle)wb.CreateCellStyle();
            style.BorderTop = style.BorderRight = style.BorderBottom = style.BorderLeft = BorderStyle.Thin;
            border = (int)BorderStyle.Thin;
            foreach (var cell in row.Cells)
              cell.CellStyle = style;

            // Conditional formatting
            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.NotBetween, "0", "5");
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Red.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"C3:C{sh.LastRowNum + 1}") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.Between, "4", "5");
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Orange.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"C3:C{sh.LastRowNum + 1}") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.Between, "3", "4");
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Yellow.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"C3:C{sh.LastRowNum + 1}") },
              rule
            );

            rule = sh.SheetConditionalFormatting.CreateConditionalFormattingRule(ComparisonOperator.Between, "0", "5");
            pf = rule.CreatePatternFormatting();
            pf.FillBackgroundColor = IndexedColors.Green.Index;
            pf.FillPattern = FillPattern.SolidForeground;
            sh.SheetConditionalFormatting.AddConditionalFormatting(
              new[] { CellRangeAddress.ValueOf($"C3:C{sh.LastRowNum + 1}") },
              rule
            );
          }

          wb.Write(outFile);
          outFile.Close();
        }
      });
    }

    private static int GetWeek(DateTime dt)
    {
      return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
  }
}
