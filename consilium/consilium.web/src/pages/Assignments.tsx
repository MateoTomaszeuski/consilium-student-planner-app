import { useEffect, useState } from 'react';
import { assignmentService } from '../services/assignmentService';
import type { Assignment, Course } from '../types';

export const Assignments = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [selectedCourse, setSelectedCourse] = useState<Course | null>(null);
  const [showAssignmentForm, setShowAssignmentForm] = useState(false);
  const [showCourseForm, setShowCourseForm] = useState(false);
  const [newCourseName, setNewCourseName] = useState('');
  const [newAssignment, setNewAssignment] = useState({
    name: '',
    description: '',
    dueDate: new Date().toISOString().split('T')[0],
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (selectedCourse) {
      filterAssignments(selectedCourse);
    }
  }, [selectedCourse]);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const [coursesData, assignmentsData] = await Promise.all([
        assignmentService.getAllCourses(),
        assignmentService.getAllAssignments(),
      ]);
      setCourses(coursesData);
      setAssignments(assignmentsData);
      if (coursesData.length > 0) {
        setSelectedCourse(coursesData[0]);
      }
    } catch (error) {
      console.error('Failed to load assignments:', error);
    }
    setIsLoading(false);
  };

  const filterAssignments = (course: Course) => {
    // Filter would happen here
    assignmentService.getAllAssignments().then(all => {
      const filtered = all.filter(a => a.courseId === course.id);
      setAssignments(filtered);
    });
  };

  const addCourse = async () => {
    if (!newCourseName.trim()) return;
    
    try {
      await assignmentService.addCourse({ courseName: newCourseName });
      setNewCourseName('');
      setShowCourseForm(false);
      await loadData();
    } catch (error) {
      console.error('Failed to add course:', error);
    }
  };

  const addAssignment = async () => {
    if (!newAssignment.name.trim() || !selectedCourse) return;

    try {
      await assignmentService.addAssignment({
        name: newAssignment.name,
        description: newAssignment.description,
        courseId: selectedCourse.id,
        dueDate: new Date(newAssignment.dueDate),
        isCompleted: false,
      });
      setNewAssignment({ name: '', description: '', dueDate: new Date().toISOString().split('T')[0] });
      setShowAssignmentForm(false);
      await loadData();
    } catch (error) {
      console.error('Failed to add assignment:', error);
    }
  };

  const deleteAssignment = async (id: number) => {
    try {
      await assignmentService.deleteAssignment(id);
      await loadData();
    } catch (error) {
      console.error('Failed to delete assignment:', error);
    }
  };

  if (isLoading) {
    return <div className="loading">Loading assignments...</div>;
  }

  return (
    <div className="assignments">
      <h1>Assignments</h1>

      <div className="course-controls">
        {!showCourseForm ? (
          <>
            <select
              value={selectedCourse?.id || ''}
              onChange={(e) => {
                const course = courses.find(c => c.id === Number(e.target.value));
                setSelectedCourse(course || null);
              }}
            >
              <option value="">Select a course</option>
              {courses.map(course => (
                <option key={course.id} value={course.id}>
                  {course.courseName}
                </option>
              ))}
            </select>
            <button onClick={() => setShowCourseForm(true)} className="btn-primary">
              New Course
            </button>
          </>
        ) : (
          <div className="course-form">
            <input
              type="text"
              placeholder="Course Name"
              value={newCourseName}
              onChange={(e) => setNewCourseName(e.target.value)}
            />
            <button onClick={addCourse} className="btn-primary">Save</button>
            <button onClick={() => setShowCourseForm(false)} className="btn-secondary">
              Cancel
            </button>
          </div>
        )}
      </div>

      {courses.length === 0 ? (
        <p className="empty-message">No courses yet. Create one to start adding assignments!</p>
      ) : (
        <>
          <button
            onClick={() => setShowAssignmentForm(!showAssignmentForm)}
            className="btn-primary"
            style={{ marginTop: '1rem' }}
          >
            Create New Assignment
          </button>

          {showAssignmentForm && (
            <div className="assignment-form">
              <h3>New Assignment</h3>
              <input
                type="text"
                placeholder="Title"
                value={newAssignment.name}
                onChange={(e) => setNewAssignment({ ...newAssignment, name: e.target.value })}
              />
              <textarea
                placeholder="Description"
                value={newAssignment.description}
                onChange={(e) => setNewAssignment({ ...newAssignment, description: e.target.value })}
              />
              <input
                type="date"
                value={newAssignment.dueDate}
                onChange={(e) => setNewAssignment({ ...newAssignment, dueDate: e.target.value })}
              />
              <div className="form-buttons">
                <button onClick={() => setShowAssignmentForm(false)} className="btn-secondary">
                  Cancel
                </button>
                <button onClick={addAssignment} className="btn-primary">Save</button>
              </div>
            </div>
          )}

          <div className="assignments-list">
            {assignments.map(assignment => (
              <div key={assignment.id} className="assignment-card">
                <h3>{assignment.name}</h3>
                {assignment.description && <p>{assignment.description}</p>}
                {assignment.dueDate && (
                  <p className="due-date">
                    Due: {assignment.dueDate.toLocaleDateString()}
                  </p>
                )}
                <button onClick={() => deleteAssignment(assignment.id)} className="btn-delete">
                  Delete
                </button>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
};
