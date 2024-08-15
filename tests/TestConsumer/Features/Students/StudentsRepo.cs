namespace TestConsumer.Features.Students;

public class StudentsRepo
{
    private readonly Dictionary<string, Student> _students = [];

    public void Add(Student student) => 
        _students.Add(student.Name, student);

    public Student? Get(string name) =>
        _students.GetValueOrDefault(name);
}
