using ClosedXML.Excel;

namespace HireHub.Core.Service;

public static class TemplateService
{

    #region Templates related to Candidate

    public static MemoryStream CandidateBulkUploadTemplate => GetCandidateBulkUploadTemplate();

    #endregion

    #region Private Methods

    private static MemoryStream GetCandidateBulkUploadTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Candidates");

        // Headers
        string[] headers =
        [
            "FullName", "Email", "Phone", "Address", "College",
            "PreviousCompany", "ExperienceLevelName", "TechStack",
            "ResumeUrl", "LinkedInUrl", "GitHubUrl"
        ];

        for (int i = 0; i < headers.Length; i++)
            worksheet.Cell(1, i + 1).Value = headers[i];

        // Header styling
        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Sample row
        worksheet.Cell(2, 1).Value = "John Doe";
        worksheet.Cell(2, 2).Value = "john.doe@example.com";
        worksheet.Cell(2, 3).Value = "9876543210";
        worksheet.Cell(2, 4).Value = "Chennai, India";
        worksheet.Cell(2, 5).Value = "Anna University";
        worksheet.Cell(2, 6).Value = "ABC Technologies";
        worksheet.Cell(2, 7).Value = "Senior";
        worksheet.Cell(2, 8).Value = "C#, SQL, Angular";
        worksheet.Cell(2, 9).Value = "https://example.com/resume.pdf";
        worksheet.Cell(2, 10).Value = "https://linkedin.com/in/";
        worksheet.Cell(2, 11).Value = "https://github.com/";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return stream;
    }

    #endregion

}
