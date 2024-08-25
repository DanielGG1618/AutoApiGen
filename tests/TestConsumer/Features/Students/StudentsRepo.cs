using System.Collections.Generic;
using System.Linq;

namespace TestConsumer.Features.Students;

public class StudentsRepo
{
    private readonly Dictionary<string, Student> _students = [];

    public void Add(Student student) => 
        _students.Add(student.Id, student);

    public Student? Get(string name) =>
        _students.GetValueOrDefault(name);

    public List<Student> GetAll() =>
        _students.Values.ToList();
}
