using System.IO;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;

namespace TimeManager.Extensions
{
  public class Test
  {
    public static async Task Do()
    {
      await Task.Run(() =>
      {
        using (var file = new FileStream(@"C:\temp\file.xlsx", FileMode.Create, FileAccess.ReadWrite))
        {
          var wb = new XSSFWorkbook();
          var sh = wb.CreateSheet("test");
          var row = sh.CreateRow(0);

          var font = wb.CreateFont();
          font.IsBold = true;
          var style = wb.CreateCellStyle();
          style.SetFont(font);

          var cell = row.CreateCell(0);
          cell.SetCellValue("Hi");
          cell.CellStyle = style;
          //cell.CellStyle.SetFont(font);

          wb.Write(file);
        }
      });

    }
  }
}
