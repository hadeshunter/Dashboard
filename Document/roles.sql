Create table Groups(
	groupid number GENERATED ALWAYS  as IDENTITY(START with 1 INCREMENT by 1),
	groupname varchar2(4000),
	active number,
	note varchar2(4000),
    primary key (groupid)
);
Create table UserPermissions(
	nguoidung_id number not null,
	groupid number not null,
	primary key (nguoidung_id,groupid),
	createday date
); 

Create table Permissions(
	perid number GENERATED ALWAYS  as IDENTITY(START with 1 INCREMENT by 1),
	pername varchar2(4000) not null,
	policy varchar2(4000) not null,
	active number,
	link varchar2(4000),
	action varchar2(1000),
	note varchar2(4000),
	position number,
	parent_id number
);
create table GroupPermissions(
	groupid number not null,
	perid number not null,
	primary key (groupid,perid),
	createday date
);


Create table PermissionTranslations(
transid number not null primary key,
perid number not null ,
languages varchar2(2000) not null,
pername varchar2(4000) not null
);



Create table Languages(
	langid number GENERATED ALWAYS  as IDENTITY(START with 1 INCREMENT by 1),
	langname varchar2(2000) not null,
	active number,
	languages varchar2(100) not null
);


select * from ADMIN_HCM.nguoidung;
