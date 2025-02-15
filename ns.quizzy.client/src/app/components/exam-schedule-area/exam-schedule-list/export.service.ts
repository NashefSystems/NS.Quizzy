import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

export interface ExportDataItem {
    examDay: string;
    startTime: string;
    subject: string;
    questionnaireName: string;
    questionnaireCode: number;
    examType: string;
    moed: string;
    duration: string;
    durationWithExtra: string;
    firstTime?: string[];
    improvement?: string[];
}


export class ExportService {
    static async exportToExcel(data: ExportDataItem[]) {
        const title = 'מערך בחינות בית ספרי';
        const columns = [
            "יום הבחינה",
            "תאריך הבחינה",
            "נושא",
            "שם שאלון",
            "מס' שאלון",
            "סוג בחינה",
            "מועד",
            "משך הבחינה",
            "ניגשים לראשונה",
            "שיפור ציון",
        ];
        const colAlignmentHorizontal: ('left' | 'center' | 'right')[] = [
            "right", //יום הבחינה,
            "right", //תאריך הבחינה,
            "right", //נושא,
            "right", //שם שאלון,
            "center", //מס' שאלון,
            "center", //סוג בחינה,
            "center", //מועד,
            "right", //משך הבחינה,
            "right", //ניגשים לראשונה,
            "right", //שיפור ציון,
        ]
        const sheetData = data.map(item => [
            item.examDay,
            item.startTime,
            item.subject,
            item.questionnaireName,
            item.questionnaireCode,
            item.examType,
            item.moed,
            item.duration === item.durationWithExtra ? item.duration : `${item.duration} [${item.durationWithExtra}]`,
            item.firstTime?.join(', '),
            item.improvement?.join(', '),
        ]);

        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet('Sheet1', { views: [{ rightToLeft: true }] });

        worksheet.columns = columns.map((col, colIndex) => {
            const colWidth = Math.max(
                col.length,
                ...sheetData.map(row => row[colIndex]?.toString().length || 2)
            );
            return {
                header: col,
                key: `c${colIndex}`,
                width: colWidth
            }
        });

        worksheet.mergeCells('A1:J1');
        const titleRow = worksheet.getCell('A1');
        titleRow.value = title;
        titleRow.font = { bold: true, size: 14, color: { argb: '000000' } };
        titleRow.alignment = { horizontal: 'center', vertical: 'middle' };
        titleRow.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FAE5D7' } };
        titleRow.border = {
            top: { style: 'thin', color: { argb: '000000' } },
            bottom: { style: 'thin', color: { argb: '000000' } },
            left: { style: 'thin', color: { argb: '000000' } },
            right: { style: 'thin', color: { argb: '000000' } },
        };

        const headerRow = worksheet.addRow(columns);
        headerRow.font = { bold: true, size: 12 };
        headerRow.alignment = { horizontal: 'center', vertical: 'middle' };
        headerRow.eachCell(cell => {
            cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FAE5D7' } };
            cell.border = {
                top: { style: 'thin', color: { argb: '000000' } },
                bottom: { style: 'thin', color: { argb: '000000' } },
                left: { style: 'thin', color: { argb: '000000' } },
                right: { style: 'thin', color: { argb: '000000' } },
            };
        });

        worksheet.autoFilter = {
            from: { row: 2, column: 1 },
            to: { row: 2, column: columns.length },
        }

        sheetData.forEach(item => {
            const row = worksheet.addRow(item);
            row.eachCell((cell, cellIndex) => {
                cell.alignment = {
                    horizontal: colAlignmentHorizontal[cellIndex - 1],
                    vertical: 'middle'
                };
                cell.border = {
                    top: { style: 'thin', color: { argb: '000000' } },
                    bottom: { style: 'thin', color: { argb: '000000' } },
                    left: { style: 'thin', color: { argb: '000000' } },
                    right: { style: 'thin', color: { argb: '000000' } },
                };
            });
        });

        const buffer = await workbook.xlsx.writeBuffer();
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${title}.xlsx`);
    }
}