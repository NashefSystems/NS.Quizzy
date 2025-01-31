import { IClassDto } from "../../../models/backend/class.dto";
import { IExamTypeDto } from "../../../models/backend/exam-type.dto";
import { IGradeDto } from "../../../models/backend/grade.dto";
import { IQuestionnaireDto } from "../../../models/backend/questionnaire.dto";

export interface ExamScheduleFilterData {
    grades: IGradeDto[];
    classes: IClassDto[];
    questionnaires: IQuestionnaireDto[];
    examTypes: IExamTypeDto[];
    filterResult: FilterResult;
}

export interface FilterResult {
    fromDate: string;
    toDate: string;
    questionnaireIds: string[];
    examTypeIds: string[];
    classIds: string[];
    gradeIds: string[];
}