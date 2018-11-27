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

 Date: 27/11/2018 20:09:13
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

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
) ENGINE = InnoDB AUTO_INCREMENT = 89 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UserAccount` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UserPassWord` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CreateTime` datetime(0) NULL,
  `UpdateTime` datetime(0) NULL,
  `CertificateNo` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Mobile` bigint(20) NOT NULL,
  `IsVerify` int(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `idx_account`(`UserAccount`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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
) ENGINE = InnoDB AUTO_INCREMENT = 89 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
