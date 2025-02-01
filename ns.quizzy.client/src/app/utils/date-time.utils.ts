import moment from "moment";

export class DateTimeUtils {
    public static getDateTimeAsIso(rawValue: string) {
        if (!rawValue) {
            return '';
        }
        return moment(rawValue).format('YYYY-MM-DDTHH:mm:ss.SSSZ')
    }


    public static getDateTimeFromIso(rawValue: string, format: string = 'YYYY-MM-DDTHH:mm') {
        if (!rawValue) {
            return '';
        }
        return moment(rawValue).format(format)
    }
}