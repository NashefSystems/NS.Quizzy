import { ClassDto } from "../../../../models/backend-models/class.dto";
import { SubjectDto } from "../../../../models/backend-models/subject.dto";

export class FilterDialogData {
    classes: ClassDto[];
    subjects: SubjectDto[];
    oldResult: FilterDialogResult;
}

export class FilterDialogResult {
    classIds: string[];
    subjectIds: string[];
} 