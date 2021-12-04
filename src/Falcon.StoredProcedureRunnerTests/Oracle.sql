--Oracle≤‚ ‘”√¥Ê¥¢π˝≥Ã
create or replace procedure GetEmployee
(
v_orgaid  IN NVARCHAR2 DEFAULT NULL
,v_data out sys_refcursor
)is
begin
    open v_data for
    select a.empcode,a.empname,a.empactive 
    from app_employee a
    where a.orgaid=v_orgaid;
     
end;
/
