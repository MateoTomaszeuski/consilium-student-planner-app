import { useEffect, useState, useCallback } from 'react';
import { assignmentService } from '../services/assignmentService';
import { useAuth } from '../hooks/useAuth';
import type { Assignment, Course } from '../types';

export const Assignments = () => {
  const { isAuthenticated } = useAuth();
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

  const loadData = useCallback(async () => {
    setIsLoading(true);
    
    if (!isAuthenticated) {
      setIsLoading(false);
      return;
    }
    
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
  }, [isAuthenticated]);

  const filterAssignments = useCallback((course: Course) => {
    // Filter would happen here
    assignmentService.getAllAssignments().then(all => {
      const filtered = all.filter(a => a.courseId === course.id);
      setAssignments(filtered);
    });
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  useEffect(() => {
    if (selectedCourse) {
      filterAssignments(selectedCourse);
    }
  }, [selectedCourse, filterAssignments]);

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
    return (
      <div className="flex items-center justify-center min-h-100">
        <div className="text-xl text-dark-dark">Loading assignments...</div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return (
      <div className="space-y-6 sm:space-y-8">
        <h1 className="text-2xl sm:text-3xl font-bold text-center text-dark-dark">Assignments</h1>
        <div className="bg-light-med border border-dark-med/30 rounded-lg p-4 sm:p-6 text-center max-w-3xl mx-auto">
          <p className="text-sm sm:text-base text-dark-dark">
            You are in Guest mode. To use all features, please go to the Profile page and log in.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto">
      <h1 className="text-2xl sm:text-3xl font-bold mb-6 sm:mb-8 text-dark-dark">Assignments</h1>

      <div className="bg-light-light border border-dark-med/20 rounded-xl p-4 sm:p-6 mb-4 sm:mb-6 shadow-sm">
        <div className="flex gap-2 sm:gap-3 items-center flex-wrap">
          {!showCourseForm ? (
            <>
              <select
                value={selectedCourse?.id || ''}
                onChange={(e) => {
                  const course = courses.find(c => c.id === Number(e.target.value));
                  setSelectedCourse(course || null);
                }}
                className="flex-1 min-w-[200px] px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
              >
                <option value="">Select a course</option>
                {courses.map(course => (
                  <option key={course.id} value={course.id}>
                    {course.courseName}
                  </option>
                ))}
              </select>
              <button 
                onClick={() => setShowCourseForm(true)} 
                className="px-4 sm:px-6 py-2 bg-mid-green hover:bg-dark-green text-white font-semibold rounded-lg transition-colors text-sm sm:text-base whitespace-nowrap"
              >
                New Course
              </button>
            </>
          ) : (
            <div className="flex gap-2 sm:gap-3 items-center flex-1 flex-wrap">
              <input
                type="text"
                placeholder="Course Name"
                value={newCourseName}
                onChange={(e) => setNewCourseName(e.target.value)}
                className="flex-1 min-w-[200px] px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
              />
              <button 
                onClick={addCourse} 
                className="px-4 sm:px-6 py-2 bg-mid-green hover:bg-dark-green text-white font-semibold rounded-lg transition-colors text-sm sm:text-base"
              >
                Save
              </button>
              <button 
                onClick={() => setShowCourseForm(false)} 
                className="px-4 sm:px-6 py-2 bg-gray-300 hover:bg-gray-400 text-dark-dark font-semibold rounded-lg transition-colors text-sm sm:text-base"
              >
                Cancel
              </button>
            </div>
          )}
        </div>
      </div>

      {courses.length === 0 ? (
        <div className="bg-light-light border border-dark-med/20 rounded-xl p-12 text-center">
          <p className="text-dark-med text-lg">No courses yet. Create one to start adding assignments!</p>
        </div>
      ) : (
        <>
          <button
            onClick={() => setShowAssignmentForm(!showAssignmentForm)}
            className="mb-4 sm:mb-6 px-4 sm:px-6 py-2 sm:py-3 bg-mid-green hover:bg-dark-green text-white font-semibold rounded-lg transition-colors shadow-sm text-sm sm:text-base"
          >
            {showAssignmentForm ? 'Cancel' : 'Create New Assignment'}
          </button>

          {showAssignmentForm && (
            <div className="bg-light-light border border-dark-med/20 rounded-xl p-4 sm:p-6 mb-4 sm:mb-6 shadow-sm">
              <h3 className="text-lg sm:text-xl font-bold mb-3 sm:mb-4 text-dark-dark">New Assignment</h3>
              <div className="space-y-3 sm:space-y-4">
                <input
                  type="text"
                  placeholder="Assignment Title"
                  value={newAssignment.name}
                  onChange={(e) => setNewAssignment({ ...newAssignment, name: e.target.value })}
                  className="w-full px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
                />
                <textarea
                  placeholder="Description"
                  value={newAssignment.description}
                  onChange={(e) => setNewAssignment({ ...newAssignment, description: e.target.value })}
                  className="w-full px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green resize-none text-sm sm:text-base"
                  rows={4}
                />
                <input
                  type="date"
                  value={newAssignment.dueDate}
                  onChange={(e) => setNewAssignment({ ...newAssignment, dueDate: e.target.value })}
                  className="w-full px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
                />
                <div className="flex gap-2 sm:gap-3 justify-end flex-wrap">
                  <button 
                    onClick={() => setShowAssignmentForm(false)} 
                    className="px-4 sm:px-6 py-2 bg-gray-300 hover:bg-gray-400 text-dark-dark font-semibold rounded-lg transition-colors text-sm sm:text-base"
                  >
                    Cancel
                  </button>
                  <button 
                    onClick={addAssignment} 
                    className="px-4 sm:px-6 py-2 bg-mid-green hover:bg-dark-green text-white font-semibold rounded-lg transition-colors text-sm sm:text-base"
                  >
                    Save
                  </button>
                </div>
              </div>
            </div>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4">
            {assignments.map(assignment => (
              <div key={assignment.id} className="bg-light-light border border-dark-med/20 rounded-xl p-4 sm:p-5 shadow-sm hover:shadow-md transition-shadow">
                <h3 className="text-base sm:text-lg font-bold text-dark-dark mb-2">{assignment.name}</h3>
                {assignment.description && (
                  <p className="text-dark-med text-xs sm:text-sm mb-3 line-clamp-3">{assignment.description}</p>
                )}
                {assignment.dueDate && (
                  <p className="text-dark-med text-xs sm:text-sm mb-3 sm:mb-4">
                    Due: {assignment.dueDate.toLocaleDateString('en-US', {
                      month: 'short',
                      day: 'numeric',
                      year: 'numeric',
                    })}
                  </p>
                )}
                <button 
                  onClick={() => deleteAssignment(assignment.id)} 
                  className="w-full px-3 sm:px-4 py-1.5 sm:py-2 text-xs sm:text-sm text-red-600 border border-red-600 rounded-lg hover:bg-red-600 hover:text-white transition-colors font-semibold"
                >
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
