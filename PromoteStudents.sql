create procedure PromoteStudents @Studies nvarchar(100), @Semester int
as
begin
	set xact_abort on;
	begin tran

	declare @IdStudies int = (select IdStudy from Studies where Name = @Studies);
	
	declare @IdEnrollment int = (select IdEnrollment from Enrollment where Semester = @Semester and IdStudy = @IdStudies);

	declare @IdEnrollmentNew int = (select IdEnrollment from Enrollment where Semester = @Semester + 1 and IdStudy = @IdStudies);
	if @IdEnrollmentNew is null
	begin
		set @IdEnrollmentNew = (select top (1) IdEnrollment + 1 from Enrollment order by IdEnrollment desc);
		insert into Enrollment values (@IdEnrollmentNew, @Semester + 1, @IdStudies, (SELECT CURRENT_TIMESTAMP))
	end

	update Student
	set IdEnrollment = @IdEnrollmentNew
	where IdEnrollment = @IdEnrollment

	select * from Enrollment where IdEnrollment = @IdEnrollmentNew

	commit
end;