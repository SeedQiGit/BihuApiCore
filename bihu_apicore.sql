/*
 Navicat Premium Data Transfer

 Source Server         : my
 Source Server Type    : MySQL
 Source Server Version : 80012
 Source Host           : localhost:3306
 Source Schema         : bihu_apicore

 Target Server Type    : MySQL
 Target Server Version : 80012
 File Encoding         : 65001

 Date: 11/05/2019 21:21:27
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for companies
-- ----------------------------
DROP TABLE IF EXISTS `companies`;
CREATE TABLE `companies`  (
  `CompId` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '公司表id',
  `Createdtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `CreatedEmp` bigint(20) NOT NULL,
  `Updatedtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  `UpdatedEmp` bigint(20) NOT NULL,
  `CompName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IsUsed` bit(1) NOT NULL COMMENT '是否可用 字典“M_IsUsed”：0不可用,1可用,2禁用,3删除',
  `CompanyType` int(11) NOT NULL COMMENT '主体类型 字典“M_SubjectType” 枚举类型EnumCompanyType  :0:车商，1:修理厂，2:专业代理，3:互联网公司，4:其他，5:保险公司',
  `PayType` int(11) NOT NULL,
  `TopAgentId` bigint(20) NOT NULL COMMENT '关联老业务的时候要用到 后期可以去掉',
  `SecretKey` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '密钥 接口客户和续保报价都需要',
  `Region` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '地域客户所在的地域 地域客户所在的地域 创建主体的时候选择',
  `PiccAccount` int(11) NOT NULL COMMENT '人保账户类别 字典“M_PiccAccKind”：0普通代理人，1人保专用',
  `ZsType` int(11) NOT NULL COMMENT '是否是车店联呼账户 字典“M_ZsType”：0不是，1保司用户，2车行用户',
  `ParentCompId` bigint(20) NOT NULL COMMENT '公司的上级ID 如果是顶级公司则是0',
  `LevelNum` int(4) NOT NULL COMMENT '层次数 从1开始 ',
  `LevelCode` varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '层次码 上级层次码+,+当前主键Id+,。顶级层次码为：“,当前主键Id,”，前后均需加逗号',
  `ClientName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`CompId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of companies
-- ----------------------------
INSERT INTO `companies` VALUES (1, '2019-04-29 13:44:29.000000', 1, '2019-04-29 13:44:36.000000', 1, 'CompName', b'1', 1, 1, 0, '1', '1', 1, 1, 0, 1, ',1,', '');

-- ----------------------------
-- Table structure for company_module_relation
-- ----------------------------
DROP TABLE IF EXISTS `company_module_relation`;
CREATE TABLE `company_module_relation`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Createdtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `CreatedEmp` bigint(20) NOT NULL,
  `Updatedtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  `UpdatedEmp` bigint(20) NOT NULL,
  `CompId` bigint(20) NOT NULL COMMENT '主体id',
  `ModuleCode` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '模块英文名称（唯一标识）',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_compId`(`CompId`) USING BTREE,
  INDEX `idx_moduleCode`(`ModuleCode`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of company_module_relation
-- ----------------------------
INSERT INTO `company_module_relation` VALUES (1, '2019-05-05 09:59:07.000000', 1, '2019-05-05 09:59:20.423043', 1, 1, 'system_all');
INSERT INTO `company_module_relation` VALUES (3, '2019-05-05 09:59:07.000000', 1, '2019-05-05 09:59:20.423043', 1, 1, 'customer_module');
INSERT INTO `company_module_relation` VALUES (5, '2019-05-05 09:59:07.000000', 1, '2019-05-05 09:59:20.423043', 1, 1, 'customer_list');

-- ----------------------------
-- Table structure for data_excel
-- ----------------------------
DROP TABLE IF EXISTS `data_excel`;
CREATE TABLE `data_excel`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `user_name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人保用户名称',
  `call_password` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '米话密码',
  `call_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '绑定号码',
  `call_ext_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '分机号  我们系统的坐席号',
  `direct_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户直线号码，只在用户需要绑定直线时使用',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 123 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of data_excel
-- ----------------------------
INSERT INTO `data_excel` VALUES (89, '陈闪仪', '1001.0', '15016168577.0', '1001.0', '0760-23870578');
INSERT INTO `data_excel` VALUES (90, '何丽荣', '1009.0', '18022018601.0', '1009.0', '0760-23870587');
INSERT INTO `data_excel` VALUES (91, '何金花', '1011.0', '13528121603.0', '1011.0', '0760-23870589');
INSERT INTO `data_excel` VALUES (92, '文大佑', '1004.0', '15007603245.0', '1004.0', '0760-23870581');
INSERT INTO `data_excel` VALUES (93, '赵婷婷', '1016.0', '13560647345.0', '1016.0', '0760-23870593');
INSERT INTO `data_excel` VALUES (94, '彭佳洁', '1017.0', '18739930717.0', '1017.0', '0760-23870595');
INSERT INTO `data_excel` VALUES (95, '蓝礼平', '1022.0', '13527161351.0', '1022.0', '0760-23870575');
INSERT INTO `data_excel` VALUES (96, '吴青', '1024.0', '15889869254.0', '1024.0', '0760-23870577');
INSERT INTO `data_excel` VALUES (97, '林华端', '1025.0', '13549861606.0', '1025.0', '0760-23870573');
INSERT INTO `data_excel` VALUES (98, '谢子聪', '1026.0', '11111111110.0', '1026.0', '0760-23870496');
INSERT INTO `data_excel` VALUES (99, '谭启文', '1027.0', '13924944132.0', '1027.0', '0760-23870497');
INSERT INTO `data_excel` VALUES (100, '黄海欣', '1028.0', '13420478231.0', '1028.0', '0760-23870498');
INSERT INTO `data_excel` VALUES (101, '朱艳玲', '1029.0', '18125336480.0', '1029.0', '0760-23870499');
INSERT INTO `data_excel` VALUES (102, '朱天鹏', '1033.0', '18565778453.0', '1033.0', '0760-23870665');
INSERT INTO `data_excel` VALUES (103, '林其其', '1032.0', '15019517954.0', '1032.0', '0760-23870662');
INSERT INTO `data_excel` VALUES (104, '陈镇科', '1043.0', '11111111118.0', '1043.0', '0760-23870659');
INSERT INTO `data_excel` VALUES (105, '梁善文', '1042.0', '11111111117.0', '1042.0', '0760-23870658');
INSERT INTO `data_excel` VALUES (106, '陈雪萍', '852456pp', '13286380281.0', '1002.0', '0760-23870579');
INSERT INTO `data_excel` VALUES (107, '徐云', '1036.0', '15800140201.0', '1036.0', '0760-23870651');
INSERT INTO `data_excel` VALUES (108, '曾细仪', '1006.0', '15913336386.0', '1006.0', '0760-23870583');
INSERT INTO `data_excel` VALUES (109, '梁家辉', '1007.0', '13631120051.0', '1007.0', '0760-23870585');
INSERT INTO `data_excel` VALUES (110, '谢海云', '1008.0', '13590921416.0', '1008.0', '0760-23870586');
INSERT INTO `data_excel` VALUES (111, '何碧妍', '1014.0', '13178609348.0', '1014.0', '0760-23870592');
INSERT INTO `data_excel` VALUES (112, '易凯', '1018.0', '13924920134.0', '1018.0', '0760-23870596');
INSERT INTO `data_excel` VALUES (113, '何灵', '1019.0', '13640427670.0', '1019.0', '0760-23870597');
INSERT INTO `data_excel` VALUES (114, '徐慧', '1020.0', '13286310005.0', '1020.0', '0760-23870598');
INSERT INTO `data_excel` VALUES (115, '陈长文', '1003.0', '18300058806.0', '1003.0', '0760-23870580');
INSERT INTO `data_excel` VALUES (116, '王明明', '1023.0', '13739106389.0', '1023.0', '0760-23870576');
INSERT INTO `data_excel` VALUES (117, '黄桂敏', '1021.0', '13715614123.0', '1021.0', '0760-23870555');
INSERT INTO `data_excel` VALUES (118, '骆伟', '1013.0', '18218938952.0', '1013.0', '0760-23870591');
INSERT INTO `data_excel` VALUES (119, '吴泽威', '1010.0', '13823945862.0', '1010.0', '0760-23870588');
INSERT INTO `data_excel` VALUES (120, '王玲', '1034.0', '16675885530.0', '1034.0', '0760-23870668');
INSERT INTO `data_excel` VALUES (121, '陈文媛', '1035.0', '15900079191.0', '1035.0', '0760-23870669');
INSERT INTO `data_excel` VALUES (122, '黄绍英', '1038.0', '15338289287.0', '1038.0', '0760-23870653');
INSERT INTO `data_excel` VALUES (123, '吴柏燕', '1012.0', '15813110972.0', '1012.0', '0760-23870590');

-- ----------------------------
-- Table structure for modules
-- ----------------------------
DROP TABLE IF EXISTS `modules`;
CREATE TABLE `modules`  (
  `ModuleCode` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '模块英文名称（唯一标识）',
  `Createdtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `CreatedEmp` bigint(20) NOT NULL,
  `Updatedtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  `UpdatedEmp` bigint(20) NOT NULL,
  `ModuleName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '模块中文名称',
  `ParentCode` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '上级模块名称',
  `ModuleLevel` int(11) NOT NULL COMMENT '模块层级 1开始',
  `ActionUrl` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ModuleType` int(11) NOT NULL COMMENT '功能类别 枚举EnumModuleType 字典“M_ModuleType”:1菜单，2按钮，3方法',
  `IsUserd` int(11) NOT NULL,
  `Icon` varchar(300) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `OrderBy` int(11) NOT NULL COMMENT '排序',
  `PlatformType` int(11) NOT NULL COMMENT 'EnumPlatformType枚举',
  PRIMARY KEY (`ModuleCode`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of modules
-- ----------------------------
INSERT INTO `modules` VALUES ('customer_list', '2019-05-05 09:57:49.365842', 1, '2019-05-05 09:57:49.365842', 1, '客户列表', 'customer_module', 2, 'Customer/List', 1, 1, NULL, 1, 1);
INSERT INTO `modules` VALUES ('customer_module', '2019-05-05 09:54:50.293441', 1, '2019-05-05 09:54:50.293441', 1, '客户管理', 'system_all', 1, ' ', 1, 1, 'icon-kehu', 1, 1);
INSERT INTO `modules` VALUES ('system_all', '2019-05-05 09:49:52.817039', 1, '2019-05-05 09:49:52.817039', 1, '根节点', '0', 0, '', 0, 1, NULL, 1, 0);

-- ----------------------------
-- Table structure for product
-- ----------------------------
DROP TABLE IF EXISTS `product`;
CREATE TABLE `product`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Price` decimal(8, 2) NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for role_module_relation
-- ----------------------------
DROP TABLE IF EXISTS `role_module_relation`;
CREATE TABLE `role_module_relation`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Createdtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `CreatedEmp` bigint(20) NOT NULL,
  `Updatedtime` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
  `UpdatedEmp` bigint(20) NOT NULL,
  `RoleId` bigint(20) NOT NULL,
  `ModuleCode` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '模块英文名称（唯一标识）',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_roleId`(`RoleId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of role_module_relation
-- ----------------------------
INSERT INTO `role_module_relation` VALUES (1, '2019-05-05 13:50:10.107932', 1, '2019-05-05 13:50:10.107932', 1, 1, 'system_all');
INSERT INTO `role_module_relation` VALUES (3, '2019-05-05 13:50:10.107932', 1, '2019-05-05 13:50:10.107932', 1, 1, 'customer_module');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UserAccount` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UserPassWord` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CreateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP(0),
  `CertificateNo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Mobile` bigint(20) NOT NULL,
  `IsVerify` int(4) NOT NULL DEFAULT 0,
  `LevelCode` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '层次码 上级层次码+,+当前主键Id+,。顶级层次码为：“,当前主键Id,”，前后均需加逗号',
  `LevelNum` int(4) NOT NULL COMMENT '层次数 从1开始',
  `ParentId` bigint(20) NOT NULL COMMENT '父级id',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_account`(`UserAccount`) USING BTREE,
  INDEX `idx_parentId`(`ParentId`) USING BTREE,
  FULLTEXT INDEX `idx_levelCode`(`LevelCode`)
) ENGINE = InnoDB AUTO_INCREMENT = 9 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (6, 'top', 'top', '123123', '2019-04-04 11:05:16', '2019-05-11 11:52:44', '123131', 13313331333, 1, ',6,', 1, 0);
INSERT INTO `user` VALUES (10, 'asd', '123', '123123', '2019-04-04 11:05:16', '2019-05-11 17:22:36', '123131', 13313331333, 1, ',6,10,', 2, 6);
INSERT INTO `user` VALUES (11, 'asd', '1233123213123', '123123', '2019-05-11 16:56:24', '2019-05-11 17:29:45', '123131', 13313331333, 1, ',6,11,', 2, 6);
INSERT INTO `user` VALUES (12, 'asd', '1233123213123', '123123', '2019-05-11 16:57:43', '2019-05-11 17:32:56', '123131', 13313331333, 1, ',6,10,12,', 3, 10);
INSERT INTO `user` VALUES (13, 'asd', '1233123213123', '123123', '2019-05-11 17:14:08', '2019-05-11 17:32:56', '123131', 13313331333, 1, ',6,10,12,13,', 4, 12);
INSERT INTO `user` VALUES (14, 'asd', '1233123213123', '123123', '2019-05-11 17:17:44', '2019-05-11 17:32:56', '123131', 13313331333, 1, ',6,10,12,14,', 4, 12);

-- ----------------------------
-- Table structure for user_config
-- ----------------------------
DROP TABLE IF EXISTS `user_config`;
CREATE TABLE `user_config`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) NOT NULL,
  `UserLevel` int(5) NOT NULL,
  `UserGrade` int(5) NOT NULL,
  `CreateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP(0),
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_userId`(`UserId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user_config
-- ----------------------------
INSERT INTO `user_config` VALUES (3, 6, 123, 123, '2019-04-04 11:05:16', '2019-04-04 11:05:16');

-- ----------------------------
-- Table structure for user_extent
-- ----------------------------
DROP TABLE IF EXISTS `user_extent`;
CREATE TABLE `user_extent`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) NOT NULL,
  `UserHobby` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UserOccupation` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CreateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP(0),
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_userId`(`UserId`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user_extent
-- ----------------------------
INSERT INTO `user_extent` VALUES (3, 6, '123123', '123131', '2019-04-04 11:05:16', '2019-04-04 11:05:16');

-- ----------------------------
-- Table structure for zs_picc_call
-- ----------------------------
DROP TABLE IF EXISTS `zs_picc_call`;
CREATE TABLE `zs_picc_call`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `user_agent_id` bigint(20) NOT NULL COMMENT '人保用户agent id',
  `user_name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '人保用户名称',
  `call_state` int(4) NOT NULL COMMENT '呼叫状态 0 禁用  1启用',
  `call_id` bigint(20) NOT NULL COMMENT '坐席号',
  `call_password` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '米话密码',
  `call_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '绑定号码',
  `CreateTime` datetime(0) NOT NULL COMMENT '创建时间',
  `UpdateTime` datetime(0) NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP(0) COMMENT '更新时间',
  `call_ext_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '分机号',
  `direct_number` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '用户直线号码，只在用户需要绑定直线时使用',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_agent_id`(`user_agent_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 126 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of zs_picc_call
-- ----------------------------
INSERT INTO `zs_picc_call` VALUES (89, 0, '陈闪仪', 1, 0, '1001', '15016168577', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1001', '0760-23870578');
INSERT INTO `zs_picc_call` VALUES (90, 0, '何丽荣', 1, 0, '1009', '18022018601', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1009', '0760-23870587');
INSERT INTO `zs_picc_call` VALUES (91, 0, '何金花', 1, 0, '1011', '13528121603', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1011', '0760-23870589');
INSERT INTO `zs_picc_call` VALUES (92, 0, '文大佑', 1, 0, '1004', '15007603245', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1004', '0760-23870581');
INSERT INTO `zs_picc_call` VALUES (93, 0, '赵婷婷', 1, 0, '1016', '13560647345', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1016', '0760-23870593');
INSERT INTO `zs_picc_call` VALUES (94, 0, '彭佳洁', 1, 0, '1017', '18739930717', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1017', '0760-23870595');
INSERT INTO `zs_picc_call` VALUES (95, 0, '蓝礼平', 1, 0, '1022', '13527161351', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1022', '0760-23870575');
INSERT INTO `zs_picc_call` VALUES (96, 0, '吴青', 1, 0, '1024', '15889869254', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1024', '0760-23870577');
INSERT INTO `zs_picc_call` VALUES (97, 0, '林华端', 1, 0, '1025', '13549861606', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1025', '0760-23870573');
INSERT INTO `zs_picc_call` VALUES (98, 0, '谢子聪', 1, 0, '1026', '11111111110', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1026', '0760-23870496');
INSERT INTO `zs_picc_call` VALUES (99, 0, '谭启文', 1, 0, '1027', '13924944132', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1027', '0760-23870497');
INSERT INTO `zs_picc_call` VALUES (100, 0, '黄海欣', 1, 0, '1028', '13420478231', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1028', '0760-23870498');
INSERT INTO `zs_picc_call` VALUES (101, 0, '朱艳玲', 1, 0, '1029', '18125336480', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1029', '0760-23870499');
INSERT INTO `zs_picc_call` VALUES (102, 0, '朱天鹏', 1, 0, '1033', '18565778453', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1033', '0760-23870665');
INSERT INTO `zs_picc_call` VALUES (103, 0, '林其其', 1, 0, '1032', '15019517954', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1032', '0760-23870662');
INSERT INTO `zs_picc_call` VALUES (104, 0, '陈镇科', 1, 0, '1043', '11111111118', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1043', '0760-23870659');
INSERT INTO `zs_picc_call` VALUES (105, 0, '梁善文', 1, 0, '1042', '11111111117', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1042', '0760-23870658');
INSERT INTO `zs_picc_call` VALUES (106, 0, '陈雪萍', 1, 0, '852456', '13286380281', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1002', '0760-23870579');
INSERT INTO `zs_picc_call` VALUES (107, 0, '徐云', 1, 0, '1036', '15800140201', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1036', '0760-23870651');
INSERT INTO `zs_picc_call` VALUES (108, 0, '曾细仪', 1, 0, '1006', '15913336386', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1006', '0760-23870583');
INSERT INTO `zs_picc_call` VALUES (109, 0, '梁家辉', 1, 0, '1007', '13631120051', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1007', '0760-23870585');
INSERT INTO `zs_picc_call` VALUES (110, 0, '谢海云', 1, 0, '1008', '13590921416', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1008', '0760-23870586');
INSERT INTO `zs_picc_call` VALUES (111, 0, '何碧妍', 1, 0, '1014', '13178609348', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1014', '0760-23870592');
INSERT INTO `zs_picc_call` VALUES (112, 0, '易凯', 1, 0, '1018', '13924920134', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1018', '0760-23870596');
INSERT INTO `zs_picc_call` VALUES (113, 0, '何灵', 1, 0, '1019', '13640427670', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1019', '0760-23870597');
INSERT INTO `zs_picc_call` VALUES (114, 0, '徐慧', 1, 0, '1020', '13286310005', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1020', '0760-23870598');
INSERT INTO `zs_picc_call` VALUES (115, 0, '陈长文', 1, 0, '1003', '18300058806', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1003', '0760-23870580');
INSERT INTO `zs_picc_call` VALUES (116, 0, '王明明', 1, 0, '1023', '13739106389', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1023', '0760-23870576');
INSERT INTO `zs_picc_call` VALUES (117, 0, '黄桂敏', 1, 0, '1021', '13715614123', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1021', '0760-23870555');
INSERT INTO `zs_picc_call` VALUES (118, 0, '骆伟', 1, 0, '1013', '18218938952', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1013', '0760-23870591');
INSERT INTO `zs_picc_call` VALUES (119, 0, '吴泽威', 1, 0, '1010', '13823945862', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1010', '0760-23870588');
INSERT INTO `zs_picc_call` VALUES (120, 0, '王玲', 1, 0, '1034', '16675885530', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1034', '0760-23870668');
INSERT INTO `zs_picc_call` VALUES (121, 0, '陈文媛', 1, 0, '1035', '15900079191', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1035', '0760-23870669');
INSERT INTO `zs_picc_call` VALUES (122, 0, '黄绍英', 1, 0, '1038', '15338289287', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1038', '0760-23870653');
INSERT INTO `zs_picc_call` VALUES (123, 0, '吴柏燕', 1, 0, '1012', '15813110972', '2018-11-27 20:45:08', '2018-11-27 20:45:07', '1012', '0760-23870590');
INSERT INTO `zs_picc_call` VALUES (124, 0, '', 1, 0, '1231231', '12312', '2019-05-08 09:05:19', '2019-05-08 09:05:19', '12312321', NULL);
INSERT INTO `zs_picc_call` VALUES (125, 0, '', 1, 0, '1231231', '12312', '2019-05-08 09:30:44', '2019-05-08 09:30:44', '12312321', NULL);
INSERT INTO `zs_picc_call` VALUES (126, 0, '', 1, 0, '1231231', '12312', '2019-05-08 09:31:28', '2019-05-08 09:31:27', '12312321', NULL);

SET FOREIGN_KEY_CHECKS = 1;
