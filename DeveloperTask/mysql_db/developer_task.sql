/*
Target Server Type    : MYSQL
Target Server Version : 50726
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for groups
-- ----------------------------
-- DROP TABLE IF EXISTS `groups`;
CREATE TABLE `groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for groupusers
-- ----------------------------
-- DROP TABLE IF EXISTS `groupusers`;
CREATE TABLE `groupusers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `group_id` int(11) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tasks
-- ----------------------------
-- DROP TABLE IF EXISTS `tasks`;
CREATE TABLE `tasks` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `author_userid` int(11) DEFAULT NULL,
  `task_name` varchar(255) DEFAULT NULL,
  `task_content` varchar(4000) DEFAULT NULL,
  `task_begin_time` datetime DEFAULT NULL,
  `task_end_time` datetime DEFAULT NULL,
  `task_status` int(11) DEFAULT NULL COMMENT '0任务预设 1任务进行 2任务已完成 3任务暂停',
  `task_is_delete` int(11) DEFAULT NULL COMMENT '0删除 1正常',
  `task_priority` int(11) DEFAULT NULL COMMENT '0普通 1优先 2紧急 数字越小优先级别越高',
  `task_create_time` datetime DEFAULT NULL,
  `task_change_time` datetime DEFAULT NULL,
  `task_developer` varchar(255) DEFAULT NULL,
  `task_type` int(11) DEFAULT NULL COMMENT '0Bug 1新增 2优化 3升级',
  `task_success_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for users
-- ----------------------------
-- DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(20) DEFAULT NULL,
  `account` varchar(30) DEFAULT NULL,
  `password` varchar(50) DEFAULT NULL,
  `weixin_openid` varchar(50) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `sex` int(1) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `is_delete` int(1) DEFAULT NULL COMMENT '0正常 1删除',
  `system_authority` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

INSERT INTO `users` (`name`, `account`, `password`, `weixin_openid`, `email`, `sex`, `create_time`, `is_delete`, `system_authority`) VALUES ('师傅', '13477030679', '4297f44b13955235245b2497399d7a93', NULL, '669247240@qq.com', '0', '2019-10-31 17:25:22', '0', '0,1,2,3,4,5,6');



CREATE TABLE `endorses` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_id` int(11) DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


CREATE TABLE `remind` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `task_id` int(11) DEFAULT NULL,
  `create_time` datetime DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;