export class TableColumnInfo {
    key: string;
    title: string;
    converter?: (value: any) => string | null | undefined;
    tooltip?: (value: any) => string | null | undefined;
}