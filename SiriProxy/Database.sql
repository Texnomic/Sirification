/*
 Navicat Premium Data Transfer

 Source Server         : Sirification
 Source Server Type    : SQLite
 Source Server Version : 3017000
 Source Schema         : main

 Target Server Type    : SQLite
 Target Server Version : 3017000
 File Encoding         : 65001

 Date: 01/08/2019 00:56:17
*/

PRAGMA foreign_keys = false;

-- ----------------------------
-- Table structure for ERRORSLOG
-- ----------------------------
DROP TABLE IF EXISTS "ERRORSLOG";
CREATE TABLE "ERRORSLOG" (
  "ENDPOINT" TEXT NOT NULL,
  "MESSAGE" TEXT NOT NULL,
  "INNERMESSAGE" TEXT NOT NULL,
  "STACKTRACE" TEXT NOT NULL,
  "DATETIME" TEXT NOT NULL
);

-- ----------------------------
-- Table structure for KEYS
-- ----------------------------
DROP TABLE IF EXISTS "KEYS";
CREATE TABLE "KEYS" (
  "ASSISTANT" TEXT NOT NULL,
  "SPEECH" TEXT NOT NULL,
  "VALIDATION" BLOB NOT NULL,
  "PLIST" BLOB NOT NULL,
  "RAWHEADER" BLOB NOT NULL,
  "HOST" TEXT NOT NULL,
  "USERAGENT" TEXT NOT NULL,
  "VALID" TEXT NOT NULL,
  "CONCURRENT" INTEGER NOT NULL,
  "LOAD" INTEGER NOT NULL,
  PRIMARY KEY ("VALIDATION")
);

-- ----------------------------
-- Table structure for SERVERLOG
-- ----------------------------
DROP TABLE IF EXISTS "SERVERLOG";
CREATE TABLE "SERVERLOG" (
  "ENDPOINT" TEXT NOT NULL,
  "PARTY" TEXT NOT NULL,
  "MESSAGE" TEXT NOT NULL
);

-- ----------------------------
-- Table structure for SETTINGS
-- ----------------------------
DROP TABLE IF EXISTS "SETTINGS";
CREATE TABLE "SETTINGS" (
  "MAXCONCURRENT" INTEGER NOT NULL,
  "MAXLOAD" INTEGER NOT NULL
);

-- ----------------------------
-- Table structure for SPEECHLOG
-- ----------------------------
DROP TABLE IF EXISTS "SPEECHLOG";
CREATE TABLE "SPEECHLOG" (
  "ENDPOINT" TEXT NOT NULL,
  "TEXT" TEXT NOT NULL
);

-- ----------------------------
-- Table structure for USERS
-- ----------------------------
DROP TABLE IF EXISTS "USERS";
CREATE TABLE "USERS" (
  "HOST" TEXT NOT NULL,
  "USERAGENT" TEXT NOT NULL,
  "EXPIREDATE" TEXT NOT NULL,
  "FIRSTNAME" TEXT,
  "LASTNAME" TEXT,
  "TWITTER" TEXT,
  "FACEBOOK" TEXT,
  "EMAIL" TEXT,
  "COMMENTS" TEXT,
  PRIMARY KEY ("HOST")
);

-- ----------------------------
-- Table structure for USERSLOG
-- ----------------------------
DROP TABLE IF EXISTS "USERSLOG";
CREATE TABLE "USERSLOG" (
  "HOST" TEXT NOT NULL,
  "USERAGENT" TEXT NOT NULL,
  "DATETIME" TEXT NOT NULL,
  "ENDPOINT" TEXT
);

PRAGMA foreign_keys = true;
