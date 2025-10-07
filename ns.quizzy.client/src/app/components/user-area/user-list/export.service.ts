import moment from 'moment';
import { Buffer } from 'buffer';
import { inject, Injectable } from '@angular/core';
import ExcelJS from 'exceljs';
import { GlobalService } from '../../../services/global.service';
import { IUserLoginStatusDto } from '../../../models/backend/user-login-status.dto';

@Injectable({
    providedIn: 'root'
})
export class ExportService {
    private readonly _globalService = inject(GlobalService);

    async exportToExcel(data: IUserLoginStatusDto[]) {
        const title = 'Quizzy login status';
        const columns = [
            "תעודת זהות",
            "שם מלא",
            "תפקיד",
            "כיתה",
            "התחברות אחרונה",
            "התראות מאופשרות"
        ];
        const colAlignmentHorizontal: ('left' | 'center' | 'right')[] = [
            "left",     // תעודת זהות
            "right",    // שם מלא
            "right",    // תפקיד            
            "center",   // כיתה
            "center",   // התחברות אחרונה
            "center",   // התראות מאופשרות
        ]
        const sheetData = data.map(item => [
            item.idNumber,
            item.fullName,
            item.role,
            item.class || '',
            !!item.lastLogin ? moment(item.lastLogin).format('YYYY-MM-DD HH:mm') : '',
            item.isAllowNotifications === 'No' ? 'לא' : item.isAllowNotifications === 'Yes' ? 'כן' : ''
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

        worksheet.mergeCells('A1:F1');
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
        const base64 = Buffer.from(buffer).toString('base64');
        this._globalService.downloadFileAsync(base64, this.getFileName(title), 'application/octet-stream');
    }

    private getFileName(title: string) {
        const createTime = moment().format('YYYY-MM-DD HH-mm');
        return `${createTime} ${title}.xlsx`;
    }
}