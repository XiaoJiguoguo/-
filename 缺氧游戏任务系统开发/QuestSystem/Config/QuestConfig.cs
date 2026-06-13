using System.Collections.Generic;
using QuestSystem.Core;

namespace QuestSystem.Config
{
    public static class QuestConfig
    {
        public static List<QuestDefinition> GetAllQuests()
        {
            var quests = new List<QuestDefinition>();

            quests.AddRange(GetNewbieQuests());
            quests.AddRange(GetDevelopmentQuests());
            quests.AddRange(GetIndustryQuests());
            quests.AddRange(GetSpaceQuests());
            quests.AddRange(GetDLCQuests());
            quests.AddRange(GetGlobalQuests());

            return quests;
        }

        private static List<QuestDefinition> GetNewbieQuests()
        {
            var list = new List<QuestDefinition>();

            list.Add(new QuestDefinition
            {
                Id = "T101",
                Name = "基础供氧系统",
                Description = "建立最基础的氧气供应系统",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Main,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "AlgaeHabitat", Description = "建造藻类箱", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "OxygenMaskLocker", Description = "建造制氧机", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "Electrolyzer", Description = "建造电解器", TargetValue = 1 }
                },
                Rewards = new List<string> { "土培砖蓝图", "基础资源包" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T102",
                Name = "初始基地建设",
                Description = "满足复制人的基本生存和卫生需求",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Main,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "Bed", Description = "建造帆布床", TargetValue = 3 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "FlushToilet", Description = "建造厕所", TargetValue = 3 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "ManualGenerator", Description = "建造人力发电机", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "MicrobeMusher", Description = "建造食物压制器", TargetValue = 1 }
                },
                Rewards = new List<string> { "规划技能点", "基础建材包" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T103",
                Name = "基础科研启动",
                Description = "建造研究站并完成第一个科技",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Main,
                Difficulty = "简单",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "ResearchCenter", Description = "建造基础研究站", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Dirt", Description = "积累泥土", TargetValue = 5000 }
                },
                Rewards = new List<string> { "加速科研" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T104",
                Name = "资源收集",
                Description = "探索周边挖掘基础资源",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Main,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Copper", Description = "收集铜矿", TargetValue = 3000 },
                    new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Sandstone", Description = "收集砂岩", TargetValue = 5000 },
                    new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Algae", Description = "收集藻类", TargetValue = 2000 }
                },
                Rewards = new List<string> { "挖掘加速", "探图范围+2" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T105",
                Name = "水源管理",
                Description = "建立蓄水池与液泵系统",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Main,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidReservoir", Description = "建造液体水库", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidPump", Description = "建造液泵", TargetValue = 1 },
                    new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Water", Description = "收集水", TargetValue = 3000 }
                },
                Rewards = new List<string> { "水管蓝图", "抽水速率+10%" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T201",
                Name = "温度控制入门",
                Description = "管理基地温度在适宜范围",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Side,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.RoomTemperatureRange, TargetId = "Barracks", Description = "营房温度 18~30℃", MinValue = 18, MaxValue = 30 }
                },
                Rewards = new List<string> { "隔热砖蓝图", "温度计" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T202",
                Name = "储物系统整理",
                Description = "建造储物箱整理基地",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Side,
                Difficulty = "简单",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "StorageLocker", Description = "建造储物箱", TargetValue = 5 }
                },
                Rewards = new List<string> { "智能储物箱蓝图" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T301",
                Name = "气体分层管理",
                Description = "检测基地氧气/二氧化碳分层",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Fault,
                Difficulty = "困难",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.RoomPressureRange, TargetId = "Barracks", Description = "营房气压保持在 1000~3000g", MinValue = 1000, MaxValue = 3000 }
                },
                Rewards = new List<string> { "气泵蓝图", "碳素脱离器蓝图" }
            });

            list.Add(new QuestDefinition
            {
                Id = "E001",
                Name = "管道冻堵修复",
                Description = "修复因液体汽化导致的管道堵塞",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Fault,
                Difficulty = "困难",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidValve", Description = "建造液体调节阀", TargetValue = 2 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidConditioner", Description = "建造液体温度调节器", TargetValue = 1 }
                },
                Rewards = new List<string> { "温控系统解锁" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T203",
                Name = "绿化基地",
                Description = "种植装饰性植物提升士气",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Side,
                Difficulty = "简单",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "PrickleFlower", Description = "种植诱饵花", TargetValue = 5 }
                },
                Rewards = new List<string> { "装饰度+5" }
            });

            list.Add(new QuestDefinition
            {
                Id = "T204",
                Name = "高压线布线",
                Description = "布置基础电力网络",
                Phase = QuestPhase.Newbie,
                Type = QuestType.Side,
                Difficulty = "普通",
                Conditions = new List<QuestCondition>
                {
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "Battery", Description = "建造电池", TargetValue = 2 },
                    new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "WireBridge", Description = "建造电线桥", TargetValue = 3 }
                },
                Rewards = new List<string> { "变压器蓝图" }
            });

            return list;
        }

        private static List<QuestDefinition> GetDevelopmentQuests()
        {
            var list = new List<QuestDefinition>
            {
                new QuestDefinition
                {
                    Id = "T106",
                    Name = "高级科研系统",
                    Description = "建造超级计算机推进科研",
                    Phase = QuestPhase.Development,
                    Type = QuestType.Main,
                    Difficulty = "普通",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "AdvancedResearchCenter", Description = "建造超级计算机", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.MinionSkillLevel, TargetId = "Learning", Description = "复制人科研技能达到6级", TargetValue = 6 }
                    },
                    Rewards = new List<string> { "中级技术包" }
                },
                new QuestDefinition
                {
                    Id = "T107",
                    Name = "自给农场",
                    Description = "建立完整食物生产链",
                    Phase = QuestPhase.Development,
                    Type = QuestType.Main,
                    Difficulty = "普通",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "PlanterBox", Description = "建造种植箱", TargetValue = 5 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "CookingStation", Description = "建造烹饪台", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Food", Description = "食物储量达20000千卡", TargetValue = 20000 }
                    },
                    Rewards = new List<string> { "冰箱蓝图", "食物保鲜时间+20%" }
                },
                new QuestDefinition
                {
                    Id = "T108",
                    Name = "管道系统标准化",
                    Description = "建立液气管路系统",
                    Phase = QuestPhase.Development,
                    Type = QuestType.Main,
                    Difficulty = "困难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.PipeConnected, TargetId = "LiquidPump", Description = "液泵接入水管网", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.PipeConnected, TargetId = "GasPump", Description = "气泵接入通气管网", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "GasFilter", Description = "建造气体过滤器", TargetValue = 1 }
                    },
                    Rewards = new List<string> { "桥接管网扩容" }
                },
                new QuestDefinition
                {
                    Id = "T109",
                    Name = "电力系统升级",
                    Description = "升级电力网络，总负载达2000W",
                    Phase = QuestPhase.Development,
                    Type = QuestType.Main,
                    Difficulty = "困难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "Generator", Description = "建造发电机", TargetValue = 3 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "Transformer", Description = "建造变压器", TargetValue = 1 }
                    },
                    Rewards = new List<string> { "智能电池蓝图" }
                },
                new QuestDefinition
                {
                    Id = "T206",
                    Name = "吸尘器清理",
                    Description = "回收基地散落物品",
                    Phase = QuestPhase.Development,
                    Type = QuestType.Side,
                    Difficulty = "简单",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "StorageLocker", Description = "储物箱填满率>80%", TargetValue = 5 }
                    },
                    Rewards = new List<string> { "自动清扫器蓝图" }
                }
            };

            return list;
        }

        private static List<QuestDefinition> GetIndustryQuests()
        {
            var list = new List<QuestDefinition>
            {
                new QuestDefinition
                {
                    Id = "T110",
                    Name = "工业生产线",
                    Description = "完成全部科技并建立工业产线",
                    Phase = QuestPhase.Industry,
                    Type = QuestType.Main,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.TechTreeComplete, TargetId = "", Description = "完成全部科技树", TargetValue = 1 }
                    },
                    Rewards = new List<string> { "全科技解锁", "工业革命成就" }
                },
                new QuestDefinition
                {
                    Id = "T111",
                    Name = "管道网络全面标准化",
                    Description = "建立全面标准化的液气管路",
                    Phase = QuestPhase.Industry,
                    Type = QuestType.Main,
                    Difficulty = "困难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidValve", Description = "建造液体调节阀", TargetValue = 5 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "GasValve", Description = "建造气体调节阀", TargetValue = 5 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidBridge", Description = "建造液体桥", TargetValue = 3 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "GasBridge", Description = "建造气体桥", TargetValue = 3 }
                    },
                    Rewards = new List<string> { "管道加速" }
                },
                new QuestDefinition
                {
                    Id = "T112",
                    Name = "温控体系",
                    Description = "建立全面温度控制系统",
                    Phase = QuestPhase.Industry,
                    Type = QuestType.Main,
                    Difficulty = "困难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LiquidConditioner", Description = "建造液温调节器", TargetValue = 2 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "GasConditioner", Description = "建造气温调节器", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "InsulationTile", Description = "建造隔热砖", TargetValue = 20 }
                    },
                    Rewards = new List<string> { "温控大师称号" }
                }
            };

            return list;
        }

        private static List<QuestDefinition> GetSpaceQuests()
        {
            return new List<QuestDefinition>
            {
                new QuestDefinition
                {
                    Id = "T113",
                    Name = "化石能源开发",
                    Description = "开采化石并建立石油化工",
                    Phase = QuestPhase.Space,
                    Type = QuestType.Main,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "OilRefinery", Description = "建造炼油厂", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Petroleum", Description = "产出石油 10000kg", TargetValue = 10000 }
                    },
                    Rewards = new List<string> { "无限能源计划" }
                },
                new QuestDefinition
                {
                    Id = "T114",
                    Name = "航天计划",
                    Description = "建造火箭发射平台",
                    Phase = QuestPhase.Space,
                    Type = QuestType.Main,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "RocketPlatform", Description = "建造火箭发射台", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.ResourceAmount, TargetId = "Steel", Description = "产出钢 5000kg", TargetValue = 5000 }
                    },
                    Rewards = new List<string> { "星际探索资格" }
                }
            };
        }

        private static List<QuestDefinition> GetDLCQuests()
        {
            return new List<QuestDefinition>();
        }

        private static List<QuestDefinition> GetGlobalQuests()
        {
            return new List<QuestDefinition>
            {
                new QuestDefinition
                {
                    Id = "H601",
                    Name = "无菌基地",
                    Description = "全周期内无小人感染疾病",
                    Phase = QuestPhase.Global,
                    Type = QuestType.Hidden,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.CycleCount, TargetId = "", Description = "存活超过100周期", TargetValue = 100 }
                    },
                    Rewards = new List<string> { "无菌基地成就", "医疗效率+30%" }
                },
                new QuestDefinition
                {
                    Id = "H602",
                    Name = "全自动化达人",
                    Description = "解锁全部科技并让所有建筑自动化",
                    Phase = QuestPhase.Global,
                    Type = QuestType.Hidden,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.TechTreeComplete, TargetId = "", Description = "完成全部科技树", TargetValue = 1 },
                        new QuestCondition { Type = QuestConditionType.BuildingCount, TargetId = "LogicWire", Description = "建造自动化线路", TargetValue = 50 }
                    },
                    Rewards = new List<string> { "自动化大师成就" }
                },
                new QuestDefinition
                {
                    Id = "E601",
                    Name = "酷暑生存",
                    Description = "在极端高温环境下维持基地运转",
                    Phase = QuestPhase.Global,
                    Type = QuestType.Emergency,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.RoomTemperatureRange, TargetId = "Barracks", Description = "全基地温度维持在 0~40℃ 之间", MinValue = 0, MaxValue = 40 }
                    },
                    Rewards = new List<string> { "酷暑勋章" }
                },
                new QuestDefinition
                {
                    Id = "E602",
                    Name = "寒冬生存",
                    Description = "在极端低温环境下维持基地运转",
                    Phase = QuestPhase.Global,
                    Type = QuestType.Emergency,
                    Difficulty = "极难",
                    IsPhaseGated = true,
                    Conditions = new List<QuestCondition>
                    {
                        new QuestCondition { Type = QuestConditionType.TemperatureThreshold, TargetId = "", Description = "室外温度恢复至10℃以上", MinValue = 10, MaxValue = 100 }
                    },
                    Rewards = new List<string> { "寒冬勋章" }
                }
            };
        }
    }
}