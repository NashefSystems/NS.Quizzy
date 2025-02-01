import { IClassDto } from "../../../models/backend/class.dto";
import { IExamTypeDto } from "../../../models/backend/exam-type.dto";
import { IGradeDto } from "../../../models/backend/grade.dto";
import { IQuestionnaireDto } from "../../../models/backend/questionnaire.dto";
import { ISubjectDto } from "../../../models/backend/subject.dto";

export interface ExamScheduleFilterData {
    grades: IGradeDto[];
    classes: IClassDto[];
    questionnaires: IQuestionnaireDto[];
    examTypes: IExamTypeDto[];
    subjects: ISubjectDto[];
    filterResult: FilterResult;
}

export interface FilterResult {
    fromDate: string;
    toDate: string;
    questionnaireIds: string[];
    examTypeIds: string[];
    classIds: string[];
    gradeIds: string[];
    subjectIds: string[];
}

export enum DialogAction { CLOSE, CLEAR, SUBMIT }

export interface DialogResult {
    action: DialogAction;
    filterResult?: FilterResult;
}