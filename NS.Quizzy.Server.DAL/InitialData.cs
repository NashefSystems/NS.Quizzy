using NS.Quizzy.Server.DAL.Entities;

namespace NS.Quizzy.Server.DAL
{
    public static class InitialData
    {
        public static readonly DateTime StartActionTime = DateTime.Now;
        public static readonly Guid SystemUserId = Guid.Parse("4320E74B-5821-4A30-94B7-CC88DDDC45EE");

        public static class UserEntityData
        {
            public static readonly User systemUser = new()
            {
                Id = SystemUserId,
                Password = "$Ns#Quizzy*(@2024&",
                FullName = "System",
                Email = "Nashef.Systems@Gmail.com",
                CreatedTime = StartActionTime,
                ModifiedTime = StartActionTime,
                IsDeleted = true,
            };
            public static readonly User adminUser = new()
            {
                Id = Guid.Parse("B900D543-90AB-4E7A-83BA-B961918DCC8C"),
                Password = "30496^#3267%$",
                FullName = "Admin",
                Email = "Nashef90@Gmail.com",
                CreatedTime = StartActionTime,
                ModifiedTime = StartActionTime,
            };
            public static readonly User sajiUser = new()
            {
                Id = Guid.Parse("2325B8AE-F12A-43D8-BE46-7041E57C9283"),
                Password = "BTWhVV8jqmP95G0w",
                FullName = "Saji Nashef",
                Email = "saji.nashef@gmail.com",
                CreatedTime = StartActionTime,
                ModifiedTime = StartActionTime,
            };
            public static List<User> GetData()
            {
                return
                [
                    systemUser,
                    adminUser,
                    sajiUser
                ];
            }
        }

        public static class ExamTypeEntityData
        {
            public static List<ExamType> GetData()
            {
                var examTypeDic = new Dictionary<string, string>()
                {
                    { "מתכונת", "DBDD71F4-D784-4ECD-B013-1D81A07C79AB"},
                    { "מתכונת I", "095CE522-087B-48C1-83EE-35541EE672F6"},
                    { "מתכונת II", "2E50B781-6DBA-4F05-8888-3B16EF618B0B"},
                    { "מתכונת III", "9ECB8ECD-4314-4A67-A843-756CDBC49296"},
                    { "בגרות", "FA73C215-82D0-4BCD-BE22-B0EABB950315"},
                    { "סימולציה", "9B936387-A52D-4256-8F44-C28F75FD15D5" }
                };
                var res = new List<ExamType>();
                for (var i = 0; i < examTypeDic.Count; i++)
                {
                    var kvp = examTypeDic.ElementAt(i);
                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Value),
                        Name = kvp.Key,
                        ItemOrder = i + 1,
                        CreatedTime = StartActionTime,
                        ModifiedTime = StartActionTime,
                    });
                }
                return res;
            }
        }

        public static class SubjectEntityData
        {
            public static List<Subject> GetData()
            {
                var subjectDic = new Dictionary<string, string>()
                {
                    {"ערבית", "9B8F8B4E-19FE-4F98-A9E1-0234D8331F24"},
                    {"עברית", "C21FE042-8AC9-4832-8E5B-04BB7DA03EED"},
                    {"אנגלית", "2F51A0AF-BBCE-4292-98F6-283628079F26"},
                    {"מתמטיקה", "4E092BA1-95F8-4DF3-A8B8-34D75175F0A0"},
                    {"אזרחות", "30C570B2-E338-4505-AB1A-480F1776F9B8"},
                    {"היסטוריה", "55F0178A-8339-47A2-AE3F-4C2B859FD52D"},
                    {"דת האיסלם", "B91976CB-943A-4BD7-8177-508C72D0FB06"},
                    {"כימיה", "F74C6FF8-47B1-4383-A932-735ADD064955"},
                    {"ביולוגיה", "81E4F556-C783-4E66-A816-751C3765E501"},
                    {"מדעי הבריאות", "9671ED50-624E-4A3B-9E25-B22D4CBDD09D"},
                    {"מדעי הסביבה", "5BD83139-BBDF-4D76-A289-BAAC22CA831C"},
                    {"תקשוב", "EBFA96FB-150D-49D3-A688-BFA1C8035014"},
                    {"מדעי המחשב", "8659E673-60E2-4FE4-9D83-C3DF37FA5B96"},
                    {"פיזיקה", "964E0673-821E-4F31-A29D-C9F587F8FB02"},
                    {"מדעי ההנדסה", "4E5A656F-13DC-4CE7-8504-CF67954C95AE"},
                    {"מערכות חשמל", "1ADFD42F-AAC6-4CA5-8458-CFEB2FD42903"},
                    {"הנדסאים", "D506C818-3300-4783-BB2E-E7005EEF269F"},
                    {"טכנאים", "EFF998D1-EB15-4C10-BD82-E7B861E71923"},
                    {"חינוך תעבורתי", "0157322F-40F3-4E07-B65A-EBC1B408E644"},

                };
                var res = new List<Subject>();
                for (var i = 0; i < subjectDic.Count; i++)
                {
                    var kvp = subjectDic.ElementAt(i);
                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Value),
                        Name = kvp.Key,
                        ItemOrder = i + 1,
                        CreatedTime = StartActionTime,
                        ModifiedTime = StartActionTime,
                    });
                }
                return res;
            }
        }

        public static class ClassEntityData
        {
            public static List<Class> GetData()
            {
                var classDic = new Dictionary<string, string>()
                {
                    { "10", "438EC849-71CB-46B0-93B6-0147B85C5564" },
                    { "1001", "E5928D27-ABBF-49B9-A34B-0329B754CD76" },
                    { "1002", "CE0BD2FF-3444-435B-A17D-03AE94936904" },
                    { "1003", "167CE96A-A9F0-4910-862A-19BC0BCDF820" },
                    { "1004", "CBEA4226-569B-4EC0-A330-26E2094FE2A7" },
                    { "1005", "F6845989-9704-4A39-A5D2-31F6A0DF9CB3" },
                    { "1006", "F093A8A6-91D2-46F4-BC8D-42A489FC75F6" },
                    { "1007", "8E1F2E5E-7597-494F-BDE9-5B290918C202" },
                    { "1008", "B096C5E7-76C6-405C-A044-5FBDBE222A3C" },
                    { "1009", "5B107D79-7DF7-4F6D-8BD4-608230492E07" },
                    { "1010", "7D88B6FE-F3F3-4282-B475-82B2908E9FE0" },
                    { "1011", "859C1031-C539-4DEB-ADFF-93BCFEC85774" },
                    { "11", "BD904A61-1129-42FB-AAB4-A6B7C3DBC037" },
                    { "1101", "494920F9-F6D9-405C-9536-AFE619749CCD" },
                    { "1102", "C3424585-63A2-4E52-940A-B8979557B6CE" },
                    { "1103", "C3F075C7-93CE-4122-AB7C-B989CF8E6786" },
                    { "1104", "21E2F08F-9793-40AA-98A0-BBF3A8759E40" },
                    { "1105", "782CB34D-7091-4E9E-9BDA-C30D11E1A64D" },
                    { "1106", "36B9D045-AC87-4C29-99AF-CCC4755B13CC" },
                    { "1107", "49211014-42B9-4486-B6A3-FB9CC7E57426" },
                    { "1108", "D514EF35-7024-4836-BE29-02E5F5B3BBCB" },
                    { "1109", "081057A2-16DF-4E2B-9E90-0CDC0DFFF8F9" },
                    { "1110", "B641E55E-5A1E-42E9-8ACB-1356340C87F5" },
                    { "1111", "2A3C8F01-ECC2-42C0-8251-16FCD68944F5" },
                    { "12", "9C9F2AAC-66E7-4AC0-A407-23698CCED44A" },
                    { "1201", "CA5695ED-4D8A-485F-9072-29DCBC29BF84" },
                    { "1202", "EBBB2B2E-4833-4860-8843-40F9CF852BE7" },
                    { "1203", "7A9808BD-2D4E-48E2-8692-67D44EF962CE" },
                    { "1204", "8C70D35A-9A3A-4BC6-8412-778F72338C63" },
                    { "1205", "F48DD2F8-9A66-4052-B31C-793E60DF5004" },
                    { "1206", "46C5B044-5020-4F7F-900C-93623BE8BBAF" },
                    { "1207", "91B2C738-8E6C-4334-BC20-9778EF792AF4" },
                    { "1208", "D22D38B9-0398-4AAE-8EA7-A13237CACE64" },
                    { "1209", "1A9C42C2-148B-4DA9-8620-A4B70336F451" },
                    { "1210", "EBB4D04C-E099-48C8-AE3C-B123354E250C" },
                    { "1211", "A238C990-8FEF-4E94-8BF4-B1E9DC818DDE" },
                    { "13", "CDE133C3-0E33-4BCB-BEE0-CC7D93AE3CB9" },
                    { "14", "2BDDC474-2779-4BB3-A73D-DF90CD8F4C24" },
                };
                var res = new List<Class>();
                for (var i = 0; i < classDic.Count; i++)
                {
                    var kvp = classDic.ElementAt(i);
                    Guid? parentId = null;
                    if (kvp.Key.Length == 4)
                    {
                        var parentClass = kvp.Key.Substring(0, 2);
                        if (classDic.TryGetValue(parentClass, out string? parentClassId))
                        {
                            parentId = Guid.Parse(parentClassId);
                        }
                    }

                    res.Add(new()
                    {
                        Id = Guid.Parse(kvp.Value),
                        Name = kvp.Key,
                        ParentId = parentId,
                        CreatedTime = StartActionTime,
                        ModifiedTime = StartActionTime,
                    });
                }
                return res;
            }
        }
    }
}

