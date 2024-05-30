drop database if exists user_registration2;
create Database user_registration2;

use user_registration2;

drop table if exists tbl_roles;
create table tbl_roles (role_id int primary key identity(1,1),
						role_name varchar(255));

insert into tbl_roles (role_name) values ('admin'),('manager'),('user');

drop table if exists tbl_permission; 
create table tbl_permission (permission_id int primary key identity(1,1),
							permsission varchar(255));

insert into tbl_permission (permsission) values ('add user'),
												('update user'),
												('delete user'),
												('show all user'),
												('view own details'),
												('chnage password'),
												('add role'),
												('add permission'),
												('add own details');



drop table if exists tbl_roles_has_permissions;
create table tbl_roles_has_permissions (role_id int foreign key references tbl_roles(role_id),
										permission int foreign key references tbl_permission(permission_id),
										primary key (role_id,permission));

insert into tbl_roles_has_permissions (role_id,permission) values (1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),
																	(2,4),(2,5),(2,6),(2,9),
																	(3,5),(3,6),(3,9);

drop table if exists tbl_user_credential; 
create table tbl_user_credential (userid int primary key identity(1,1),
					username varchar(255),
					password varchar(255),
					salt varchar(4),
					role int foreign key references tbl_roles(role_id) default 1,
					is_active tinyint default 1);


drop table if exists tbl_user_detail; 
create table tbl_user_detail (userid int foreign key references tbl_user_credential(userid),
					firstname varchar(255),
					lastname varchar(255),
					email varchar(255),
					mobileno varchar(10),
					city varchar(255),
					state varchar(255));

drop procedure if exists create_user; 
go 
create procedure create_user @username varchar(255),
								@password varchar(255),
								@salt varchar(4),
								@role int
as
	begin
		set @password= (select CONVERT(varchar(32),HASHBYTES('md5',CONCAT(@password,@salt)),2));
		insert into tbl_user_credential (username,password,salt,role) values 
		(@username,@password,@salt,@role);
	end;
go

exec create_user 'admin123','admin123','abcd','1';

drop procedure if exists check_user; 
go 
create procedure check_user @username varchar(255),
								@password varchar(255)
as
	begin
		set @password= (select CONVERT(varchar(32),HASHBYTES('md5',CONCAT(@password,'abcd')),2));
			select tbl_user_credential.userid,tbl_user_credential.role,tbl_roles_has_permissions.permission as permissionid,tbl_permission.permsission 
			from tbl_user_credential left join tbl_roles_has_permissions on tbl_user_credential.role=tbl_roles_has_permissions.role_id left join 
			tbl_permission on tbl_permission.permission_id=tbl_roles_has_permissions.permission where tbl_user_credential.username=@username
			And tbl_user_credential.password=@password and tbl_user_credential.is_active=1; 
	end;
go

exec check_user 'admin123','admin123';

drop procedure if exists add_own_details; 
go 
create procedure add_own_details @fname varchar(255),
								@lname varchar(255),
								@email varchar(255),
								@mobileno varchar(10),
								@city varchar(255),
								@state varchar(255),
								@userid int
as
	begin
		insert into tbl_user_detail(userid,firstname,lastname,email,mobileno,city,state) values 
		(@userid,@fname,@lname,@email,@mobileno,@city,@state); 
	end;
go

drop procedure if exists show_users; 
go 
create procedure show_users @role int
as
	begin
		if @role=1
		select tbl_user_credential.userid,tbl_user_credential.username,tbl_roles.role_name,tbl_user_detail.firstname,tbl_user_detail.lastname 
		from tbl_user_credential left join tbl_user_detail on tbl_user_credential.userid=tbl_user_detail.userid left join tbl_roles on tbl_roles.role_id=tbl_user_credential.role where tbl_user_credential.is_active=1;
		else if @role=2
		select tbl_user_credential.userid,tbl_user_credential.username,tbl_roles.role_name,tbl_user_detail.firstname,tbl_user_detail.lastname 
		from tbl_user_credential left join tbl_user_detail on tbl_user_credential.userid=tbl_user_detail.userid  left join tbl_roles on tbl_roles.role_id=tbl_user_credential.role where tbl_user_credential.is_active=1 and role=3;
	end;
go

exec show_users '2';

drop procedure if exists show_own_detail; 
go 
create procedure show_own_detail @userid int
as
	begin
		select tbl_user_credential.username,tbl_user_detail.firstname,tbl_user_detail.lastname,tbl_user_detail.mobileno,tbl_user_detail.email,tbl_user_detail.city,tbl_user_detail.state 
		from tbl_user_credential left join tbl_user_detail on tbl_user_credential.userid=tbl_user_detail.userid where tbl_user_credential.is_active=1 and tbl_user_credential.userid=@userid;
		end;
go

exec show_own_detail '1';

drop procedure if exists change_password; 
go 
create procedure change_password @userid int,
								@password varchar(255)
as
	begin
		set @password= (select CONVERT(varchar(32),HASHBYTES('md5',CONCAT(@password,'abcd')),2));
		update tbl_user_credential set password=@password where userid=@userid;
		end;
go

drop procedure if exists delete_user; 
go 
create procedure delete_user @userid int
as
	begin
		update tbl_user_credential set is_active=0 where userid=@userid;
		end;
go

drop procedure if exists update_user; 
go 
create procedure update_user @fname varchar(255),
								@lname varchar(255),
								@email varchar(255),
								@mobileno varchar(10),
								@city varchar(255),
								@state varchar(255),
								@userid int
as
	begin
		if exists(select userid from tbl_user_detail where userid=@userid)
		update tbl_user_detail set firstname=@fname,lastname=@lname,mobileno=@mobileno,
		email=@email,city=@city,state=@state where userid=@userid;
		else
			insert into tbl_user_detail(userid,firstname,lastname,email,mobileno,city,state) values 
			(@userid,@fname,@lname,@email,@mobileno,@city,@state);
	end;
go

drop procedure if exists show_role_permission;
go 
create procedure show_role_permission @role int
as
	begin
		 select tbl_roles_has_permissions.permission as permissionid,tbl_permission.permsission from tbl_roles_has_permissions left join tbl_permission on
		 tbl_roles_has_permissions.permission=tbl_permission.permission_id where tbl_roles_has_permissions.role_id=@role
		end;
go

drop procedure if exists show_available_permission;
go 
create procedure show_available_permission
as
	begin
		select * from tbl_permission;
		end;
go

exec show_available_permission;

exec show_role_permission 1;

drop procedure if exists change_role_permission;
go 
create procedure change_role_permission @role int,
										@change int,
										@prid int 
as
	begin
		if @change=1
			insert into tbl_roles_has_permissions (role_id,permission) values (@role,@prid);
		else
			delete from tbl_roles_has_permissions where tbl_roles_has_permissions.permission=@prid and tbl_roles_has_permissions.role_id=@role;
		end;
go

exec change_role_permission 1,1,9;

drop table if exists tbl_user_has_special_permission; 
create table tbl_user_has_special_permission (userid int foreign key references tbl_user_credential(userid),
												permission int foreign key references tbl_permission(permission_id),
										primary key (userid,permission));


drop procedure if exists give_user_permission;
go 
create procedure give_user_permission @userid int,
										@change int,
										@prid int 
as
	begin
		if @change=1
			insert into tbl_user_has_special_permission (userid,permission) values (@userid,@prid);
		else
			delete from tbl_user_has_special_permission where tbl_user_has_special_permission.permission=@prid and tbl_user_has_special_permission.userid=@userid;
		end;
go

exec give_user_permission 5,1,2;

drop procedure if exists show_user_permission;
go 
create procedure show_user_permission @userid int
as
	begin
		select tbl_user_has_special_permission.permission as permissionid,tbl_permission.permsission from tbl_user_has_special_permission left join tbl_permission
		on tbl_user_has_special_permission.permission=tbl_permission.permission_id where tbl_user_has_special_permission.userid=@userid;
	end;
go

exec show_user_permission 5;