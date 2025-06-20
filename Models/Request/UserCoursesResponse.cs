public class UserCoursesResponse
{
    public List<UserCourseDto> OwnedCourses { get; set; } = new List<UserCourseDto>();
    public List<UserCourseDto> TeachingCourses { get; set; } = new List<UserCourseDto>();
    public List<UserCourseDto> StudentCourses { get; set; } = new List<UserCourseDto>();
}