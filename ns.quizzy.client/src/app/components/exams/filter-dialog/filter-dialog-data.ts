import { IClassDto } from "../../../models/backend/class.dto";
import { ISubjectDto } from "../../../models/backend/subject.dto";

export class FilterDialogData {
    classes: IClassDto[];
    subjects: ISubjectDto[];
    oldResult: FilterDialogResult;
}

export class FilterDialogResult {
    classIds: string[];
    subjectIds: string[];
} 