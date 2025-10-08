using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL.Entities;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.DAL
{
    internal static class InitialData
    {
        internal static class UserEntityData
        {
            internal static readonly User systemUser = new()
            {
                Id = Guid.Parse("4320E74B-5821-4A30-94B7-CC88DDDC45EE"),
                Password = "$Ns#Quizzy*(@2024&",
                FullName = "System",
                Email = "Nashef.Systems@Gmail.com",
                Role = DALEnums.Roles.SuperAdmin,
                TwoFactorSecretKey = "97F1AFCD316343B4B2D492A10B036680",
                IsDeleted = true,
            };

            internal static readonly User adminUser = new()
            {
                Id = Guid.Parse("B900D543-90AB-4E7A-83BA-B961918DCC8C"),
                Password = "304963267",
                FullName = "Admin",
                Email = "Nashef90@Gmail.com",
                TwoFactorSecretKey = "XD2GB3DYXAGZGGOXG46TD3QKBQXQYYKO",
                NotificationToken = "eiP6Nt2PTK-oE-gd2hD_0a:APA91bGCpOImJoDUpEbrRBqPWqu50Z-bx90hdVLEhscx1og2FPtPt5Xn1UMT0siClSbXpAMdeie6d41FxnJtUpEPEzaU7p2tlMETwyng_YT4kEQ5sBd-0Zs",
                Role = DALEnums.Roles.Developer,
            };

            internal static readonly User sajiUser = new()
            {
                Id = Guid.Parse("2325B8AE-F12A-43D8-BE46-7041E57C9283"),
                Password = "4557969",
                FullName = "Saji Nashef",
                Email = "saji.nashef@gmail.com",
                TwoFactorSecretKey = "L2PUPNK2U5SIIDHZUPWW6HHYRY7ZQSYX",
                NotificationToken = "eeCTLExNRcGRRTmz1yXhue:APA91bHtF4PgtFbLsqAJHDw8-koBVRjZBCy6A_15OI_NfavWtvQwtytXsGQDoVa9Y_w__nap_jgQkOoA_YYAVM6UyKpeONooPgfCd4y5CItcIfUOizkmvHU",
                Role = DALEnums.Roles.Admin,
            };

            internal static readonly User DemoUser = new()
            {
                Id = Guid.Parse("F8178F51-6D73-45E1-A4E7-1B01BAF9884D"),
                Password = "B@Juor*W@Bg8CHX-PqwJwhio2!XcELsn",
                FullName = "Demo user",
                Email = "QuizzyDemo@ExamProduction.com",
                TwoFactorSecretKey = "USUYJXW3QXUHA7R53YP4QBPPXHH54KHS",
                NotificationToken = "eiP6Nt2PTK-oE-gd2hD_0a:APA91bGCpOImJoDUpEbrRBqPWqu50Z-bx90hdVLEhscx1og2FPtPt5Xn1UMT0siClSbXpAMdeie6d41FxnJtUpEPEzaU7p2tlMETwyng_YT4kEQ5sBd-0Zs",
                IdNumber = "000000018",
                Role = DALEnums.Roles.Student,
            };

            internal static List<User> GetData()
            {
                return
                [
                    systemUser,
                    adminUser,
                    sajiUser,
                    DemoUser,
                ];
            }
        }

        internal static class ExamTypeEntityData
        {
            internal static List<ExamType> GetData()
            {
                var examTypeDic = new Dictionary<string, string>()
                {
                    { "DBDD71F4-D784-4ECD-B013-1D81A07C79AB", "מתכונת" },
                    { "095CE522-087B-48C1-83EE-35541EE672F6", "מתכונת I" },
                    { "2E50B781-6DBA-4F05-8888-3B16EF618B0B", "מתכונת II" },
                    { "9ECB8ECD-4314-4A67-A843-756CDBC49296", "מתכונת III" },
                    { "FA73C215-82D0-4BCD-BE22-B0EABB950315", "בגרות" },
                    { "9B936387-A52D-4256-8F44-C28F75FD15D5", "סימולציה" },
                };
                var res = new List<ExamType>();
                for (var i = 0; i < examTypeDic.Count; i++)
                {
                    var kvp = examTypeDic.ElementAt(i);
                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Key),
                        Name = kvp.Value,
                        ItemOrder = i + 1,
                    });
                }
                return res;
            }
        }

        internal static class MoedEntityData
        {
            internal static List<Moed> GetData()
            {
                var moedDic = new Dictionary<string, string>()
                {
                    { "72CDE11D-B3F6-47F2-9909-51BCD30FF086", "חורף" },
                    { "1E33F673-6773-4DB1-B6CF-909586A6544B", "אביב" },
                    { "159BB5E6-9460-4E43-BA3C-B5967CF99F4E", "קיץ" },
                    { "1C3A5BE0-9727-4A9D-B525-1461F33DED8F", "קיץ מועד ב'" },
                };
                var res = new List<Moed>();
                for (var i = 0; i < moedDic.Count; i++)
                {
                    var kvp = moedDic.ElementAt(i);
                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Key),
                        Name = kvp.Value,
                        ItemOrder = i + 1,
                    });
                }
                return res;
            }
        }

        internal static class SubjectEntityData
        {
            internal static List<Subject> GetData()
            {
                var subjectDic = new Dictionary<string, string>()
                {
                    { "9B8F8B4E-19FE-4F98-A9E1-0234D8331F24", "ערבית" },
                    { "C21FE042-8AC9-4832-8E5B-04BB7DA03EED", "עברית" },
                    { "2F51A0AF-BBCE-4292-98F6-283628079F26", "אנגלית" },
                    { "4E092BA1-95F8-4DF3-A8B8-34D75175F0A0", "מתמטיקה" },
                    { "30C570B2-E338-4505-AB1A-480F1776F9B8", "אזרחות" },
                    { "55F0178A-8339-47A2-AE3F-4C2B859FD52D", "היסטוריה" },
                    { "B91976CB-943A-4BD7-8177-508C72D0FB06", "דת האיסלם" },
                    { "F74C6FF8-47B1-4383-A932-735ADD064955", "כימיה" },
                    { "81E4F556-C783-4E66-A816-751C3765E501", "ביולוגיה" },
                    { "9671ED50-624E-4A3B-9E25-B22D4CBDD09D", "מדעי הבריאות" },
                    { "5BD83139-BBDF-4D76-A289-BAAC22CA831C", "מדעי הסביבה" },
                    { "EBFA96FB-150D-49D3-A688-BFA1C8035014", "תקשוב" },
                    { "8659E673-60E2-4FE4-9D83-C3DF37FA5B96", "מדעי המחשב" },
                    { "964E0673-821E-4F31-A29D-C9F587F8FB02", "פיזיקה" },
                    { "4E5A656F-13DC-4CE7-8504-CF67954C95AE", "מדעי ההנדסה" },
                    { "1ADFD42F-AAC6-4CA5-8458-CFEB2FD42903", "מערכות חשמל" },
                    { "D506C818-3300-4783-BB2E-E7005EEF269F", "הנדסאים" },
                    { "EFF998D1-EB15-4C10-BD82-E7B861E71923", "טכנאים" },
                    { "0157322F-40F3-4E07-B65A-EBC1B408E644", "חינוך תעבורתי" },

                };
                var res = new List<Subject>();
                for (var i = 0; i < subjectDic.Count; i++)
                {
                    var kvp = subjectDic.ElementAt(i);
                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Key),
                        Name = kvp.Value,
                        ItemOrder = i + 1,
                    });
                }
                return res;
            }
        }

        internal static class GradeEntityData
        {
            internal static Guid GRADE_10_ID = Guid.Parse("438EC849-71CB-46B0-93B6-0147B85C5564");
            internal static Guid GRADE_11_ID = Guid.Parse("BD904A61-1129-42FB-AAB4-A6B7C3DBC037");
            internal static Guid GRADE_12_ID = Guid.Parse("9C9F2AAC-66E7-4AC0-A407-23698CCED44A");
            internal static Guid GRADE_13_ID = Guid.Parse("CDE133C3-0E33-4BCB-BEE0-CC7D93AE3CB9");
            internal static Guid GRADE_14_ID = Guid.Parse("2BDDC474-2779-4BB3-A73D-DF90CD8F4C24");
            internal static List<Grade> GetData()
            {
                return [
                    new(){ Id = GRADE_10_ID, Code = 10, Name = "שכבה י'" },
                    new(){ Id = GRADE_11_ID, Code = 11, Name = "שכבה י\"א" },
                    new(){ Id = GRADE_12_ID, Code = 12, Name = "שכבה י\"ב" },
                    new(){ Id = GRADE_13_ID, Code = 13, Name = "שכבה י\"ג" },
                    new(){ Id = GRADE_14_ID, Code = 14, Name = "שכבה י\"ד" },
                ];
            }
        }

        internal static class ClassEntityData
        {
            internal static List<Class> GetData()
            {
                return
                [
                    new(){
                        Id = Guid.Parse("E5928D27-ABBF-49B9-A34B-0329B754CD76"),
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Code = 1,
                        Name = "י' 1"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("CE0BD2FF-3444-435B-A17D-03AE94936904"),
                        Code = 2,
                        Name = "י' 2"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("167CE96A-A9F0-4910-862A-19BC0BCDF820"),
                        Code = 3,
                        Name = "י' 3"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("CBEA4226-569B-4EC0-A330-26E2094FE2A7"),
                        Code = 4,
                        Name = "י' 4"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("F6845989-9704-4A39-A5D2-31F6A0DF9CB3"),
                        Code = 5,
                        Name = "י' 5"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("F093A8A6-91D2-46F4-BC8D-42A489FC75F6"),
                        Code = 6,
                        Name = "י' 6"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("8E1F2E5E-7597-494F-BDE9-5B290918C202"),
                        Code = 7,
                        Name = "י' 7"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("B096C5E7-76C6-405C-A044-5FBDBE222A3C"),
                        Code = 8,
                        Name = "י' 8"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("5B107D79-7DF7-4F6D-8BD4-608230492E07"),
                        Code = 9,
                        Name = "י' 9"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("7D88B6FE-F3F3-4282-B475-82B2908E9FE0"),
                        Code = 10,
                        Name = "י' 10"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_10_ID,
                        Id = Guid.Parse("859C1031-C539-4DEB-ADFF-93BCFEC85774"),
                        Code = 11,
                        Name = "י' 11"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("494920F9-F6D9-405C-9536-AFE619749CCD"),
                        Code = 1,
                        Name = "י\"א 1"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("C3424585-63A2-4E52-940A-B8979557B6CE"),
                        Code = 2,
                        Name = "י\"א 2"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("C3F075C7-93CE-4122-AB7C-B989CF8E6786"),
                        Code = 3,
                        Name = "י\"א 3"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("21E2F08F-9793-40AA-98A0-BBF3A8759E40"),
                        Code = 4,
                        Name = "י\"א 4"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("782CB34D-7091-4E9E-9BDA-C30D11E1A64D"),
                        Code = 5,
                        Name = "י\"א 5"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("36B9D045-AC87-4C29-99AF-CCC4755B13CC"),
                        Code = 6,
                        Name = "י\"א 6"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("49211014-42B9-4486-B6A3-FB9CC7E57426"),
                        Code = 7,
                        Name = "י\"א 7"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("D514EF35-7024-4836-BE29-02E5F5B3BBCB"),
                        Code = 8,
                        Name = "י\"א 8"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("081057A2-16DF-4E2B-9E90-0CDC0DFFF8F9"),
                        Code = 9,
                        Name = "י\"א 9"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("B641E55E-5A1E-42E9-8ACB-1356340C87F5"),
                        Code = 10,
                        Name = "י\"א 10"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_11_ID,
                        Id = Guid.Parse("2A3C8F01-ECC2-42C0-8251-16FCD68944F5"),
                        Code = 11,
                        Name = "י\"א 11"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("CA5695ED-4D8A-485F-9072-29DCBC29BF84"),
                        Code = 1,
                        Name = "י\"ב 1"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("EBBB2B2E-4833-4860-8843-40F9CF852BE7"),
                        Code = 2,
                        Name = "י\"ב 2"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("7A9808BD-2D4E-48E2-8692-67D44EF962CE"),
                        Code = 3,
                        Name = "י\"ב 3"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("8C70D35A-9A3A-4BC6-8412-778F72338C63"),
                        Code = 4,
                        Name = "י\"ב 4"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("F48DD2F8-9A66-4052-B31C-793E60DF5004"),
                        Code = 5,
                        Name = "י\"ב 5"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("46C5B044-5020-4F7F-900C-93623BE8BBAF"),
                        Code = 6,
                        Name = "י\"ב 6"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("91B2C738-8E6C-4334-BC20-9778EF792AF4"),
                        Code = 7,
                        Name = "י\"ב 7"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("D22D38B9-0398-4AAE-8EA7-A13237CACE64"),
                        Code = 8,
                        Name = "י\"ב 8"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("1A9C42C2-148B-4DA9-8620-A4B70336F451"),
                        Code = 9,
                        Name = "י\"ב 9"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("EBB4D04C-E099-48C8-AE3C-B123354E250C"),
                        Code = 10,
                        Name = "י\"ב 10"
                    },
                    new(){
                        GradeId = GradeEntityData.GRADE_12_ID,
                        Id = Guid.Parse("A238C990-8FEF-4E94-8BF4-B1E9DC818DDE"),
                        Code = 11,
                        Name = "י\"ב 11"
                    },
                ];
            }
        }

        internal static class AppSettingEntityData
        {
            internal static List<AppSetting> GetData()
            {
                return
                [
                    new()
                    {
                        Id = Guid.Parse("795B0B48-4238-4D27-BE60-3DD2CB66E424"),
                        Key = AppSettingKeys.SavePasswordOnRememberMe.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Client,
                        ValueType = DALEnums.AppSettingValueTypes.Boolean,
                        Value = "true",
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("4D9C6810-644D-4AC4-B00F-245AA2B69086"),
                        Key =  AppSettingKeys.CacheDataTTLMin.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Integer,
                        Value = "300", // 5 Hours
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("586641C6-6289-4DCF-BC0B-0F5513CBD911"),
                        Key =  AppSettingKeys.CacheLoginsTTLMin.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Integer,
                        Value = "20160", // 2 Week
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("4AAA77C9-37E3-417D-8549-0C541869CA6C"),
                        Key =  AppSettingKeys.ServerInfoTTLMin.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Integer,
                        Value = "20160", // 2 Week
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("C34BF183-1FF4-4B96-8524-076243AAD56B"),
                        Key =  AppSettingKeys.CacheOTPTTLMin.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Integer,
                        Value = "60", // 1 Hour
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("4F43FB54-4BBD-4F04-8DFE-14BD5078946E"),
                        Key =  AppSettingKeys.Email.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.All,
                        ValueType = DALEnums.AppSettingValueTypes.String,
                        Value = "Quizzy@ExamProduction.com",
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("795B147F-6DE4-44E1-BD09-6B2B778FC2D3"),
                        Key =  AppSettingKeys.IdNumberEmailDomain.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.All,
                        ValueType = DALEnums.AppSettingValueTypes.String,
                        Value = "Quizzy.ExamProduction.com",
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("648308A0-690F-45E7-B4A8-C0F0680625AC"),
                        Key =  AppSettingKeys.IgnoreOTPValidationUserIds.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Json,
                        Value = "[]",
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("A36301B7-8FDD-4677-9E49-13C17614AB07"),
                        Key =  AppSettingKeys.NotificationsGetLimitValue.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Integer,
                        Value = "50",
                        IsSecured = false,
                        IsDeleted = false,
                    },
                    new()
                    {
                        Id = Guid.Parse("D57B4DC7-D933-477F-8CD4-16C7FAB8B9FF"),
                        Key =  AppSettingKeys.GoogleCredentialJson.GetDBStringValue(),
                        Target = DALEnums.AppSettingTargets.Server,
                        ValueType = DALEnums.AppSettingValueTypes.Json,
                        Value = "{}",
                        IsSecured = false,
                        IsDeleted = false,
                    }
                ];
            }
        }
    }
}

