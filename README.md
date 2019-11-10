# taskmanagement
关键字：任务管理、BUG管理<br>
该网站适用于开发团队的日常任务分配管理和BUG跟踪管理<br><br>
该项目采用：.NET MVC5+MYSQL<br>


功能模块介绍：<br>
1.任务管理模块<br>
拥有该模块权限的用户可以创建、修改、删除任务。<br><br>
2.员工管理模块<br>
拥有该模块权限的用户可以添加、修改、删除员工。其中还可以设置员工的权限，权限包含：修改密码、创建任务、管理任务、添加员工、员工管理、创建小组、小组管理。可以根据不同用户角色设置不同管理权限。
<br><br>
3.小组管理模块<br>
拥有该模块权限的用户可以添加、修改、删除小组。其中在创建或修改小组的时候可以设置该小组的成员。目前该模块尚未发展出应有的功效日后有待开发。
<br><br>
规则介绍<br>
当员工拥有发布任务的权力时<br>
A发布一条任务，如果没有设置执行人，那么A发布的这条任务只能在A自己的【任务管理】中管理<br>
A发布一条任务，如果设置执行人A和B，那么A和B可以在自己的【我的任务】看到这条任务，同时A可以在自己的【任务管理】中心管理
<br><br>
任务优先级说明<br>
默认按照：紧急、优先、普通排序，其次按照任务的开始时间降序排序，再其次按照任务的创建时间和最后修改时间降序排序
