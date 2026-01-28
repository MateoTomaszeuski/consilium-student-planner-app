import { useState, useRef, useEffect, useCallback } from 'react';
import { useAppStore } from '../store/appStore';
import { useAuth } from '../hooks/useAuth';
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';

type Tool = 'Notes' | 'Calculator' | 'Pomodoro' | 'Calendar';

export const Tools = () => {
  const { isAuthenticated } = useAuth();
  const [activeTool, setActiveTool] = useState<Tool>('Notes');

  if (!isAuthenticated) {
    return (
      <div className="space-y-6 sm:space-y-8">
        <h1 className="text-2xl sm:text-3xl font-bold text-center text-dark-dark">Tools</h1>
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
      <h1 className="text-2xl sm:text-3xl font-bold mb-6 sm:mb-8 text-dark-dark text-center">Tools</h1>
      
      <div className="flex gap-2 sm:gap-3 mb-6 sm:mb-8 justify-center flex-wrap">
        <button
          className={`px-4 sm:px-6 py-2 sm:py-3 rounded-lg font-semibold transition-all text-sm sm:text-base ${
            activeTool === 'Notes' 
              ? 'bg-mid-green text-white shadow-md' 
              : 'bg-light-light text-dark-dark border border-dark-med/20 hover:border-mid-green'
          }`}
          onClick={() => setActiveTool('Notes')}
        >
          Notes
        </button>
        <button
          className={`px-4 sm:px-6 py-2 sm:py-3 rounded-lg font-semibold transition-all text-sm sm:text-base ${
            activeTool === 'Calculator' 
              ? 'bg-mid-green text-white shadow-md' 
              : 'bg-light-light text-dark-dark border border-dark-med/20 hover:border-mid-green'
          }`}
          onClick={() => setActiveTool('Calculator')}
        >
          Calculator
        </button>
        <button
          className={`px-4 sm:px-6 py-2 sm:py-3 rounded-lg font-semibold transition-all text-sm sm:text-base ${
            activeTool === 'Pomodoro' 
              ? 'bg-mid-green text-white shadow-md' 
              : 'bg-light-light text-dark-dark border border-dark-med/20 hover:border-mid-green'
          }`}
          onClick={() => setActiveTool('Pomodoro')}
        >
          Pomodoro
        </button>
        <button
          className={`px-4 sm:px-6 py-2 sm:py-3 rounded-lg font-semibold transition-all text-sm sm:text-base ${
            activeTool === 'Calendar' 
              ? 'bg-mid-green text-white shadow-md' 
              : 'bg-light-light text-dark-dark border border-dark-med/20 hover:border-mid-green'
          }`}
          onClick={() => setActiveTool('Calendar')}
        >
          Calendar
        </button>
      </div>

      <div className="bg-light-light rounded-xl p-4 sm:p-8 border border-dark-med/20 shadow-sm">
        {activeTool === 'Notes' && <NotesView />}
        {activeTool === 'Calculator' && <CalculatorView />}
        {activeTool === 'Pomodoro' && <PomodoroView />}
        {activeTool === 'Calendar' && <CalendarView />}
      </div>
    </div>
  );
};

const NotesView = () => {
  const { notes, setNotes } = useAppStore();
  const { user, updateNotes } = useAuth();
  const [content, setContent] = useState(() => {
    return user?.notes || notes.content || '';
  });
  const [isSaving, setIsSaving] = useState(false);

  // Sync initial notes to store if loaded from user
  useEffect(() => {
    if (user?.notes && user.notes !== notes.content) {
      setNotes({ content: user.notes });
    }
  }, [setNotes, notes.content, user]);

  const handleChange = async (value: string) => {
    setContent(value);
    setNotes({ content: value });
    
    // Debounce the save to backend
    setIsSaving(true);
    await updateNotes(value);
    setIsSaving(false);
  };

  return (
    <div className="w-full">
      <div className="flex items-center justify-between mb-3 sm:mb-4">
        <h2 className="text-xl sm:text-2xl font-bold text-dark-dark">Quick Notes</h2>
        {isSaving && <span className="text-xs sm:text-sm text-dark-med">Saving...</span>}
      </div>
      <textarea
        value={content}
        onChange={(e) => handleChange(e.target.value)}
        placeholder="Start typing your notes..."
        className="w-full h-48 sm:h-64 p-3 sm:p-4 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green resize-y overflow-auto min-h-40 max-h-screen text-sm sm:text-base"
      />
    </div>
  );
};

const CalculatorView = () => {
  const [display, setDisplay] = useState('0');
  const [currentInput, setCurrentInput] = useState('');
  const [previousValue, setPreviousValue] = useState(0);
  const [operation, setOperation] = useState('');
  const [expression, setExpression] = useState('');

  const handleDigit = (digit: string) => {
    const newInput = currentInput + digit;
    const newExpression = expression + digit;
    setCurrentInput(newInput);
    setExpression(newExpression);
    setDisplay(newExpression);
  };

  const handleOperator = (op: string) => {
    if (currentInput) {
      setPreviousValue(parseFloat(currentInput));
      setCurrentInput('');
    }
    setOperation(op);
    setExpression(expression + op);
    setDisplay(expression + op);
  };

  const handleEquals = () => {
    if (!currentInput || !operation) return;
    
    const current = parseFloat(currentInput);
    let result = 0;
    
    switch (operation) {
      case '+':
        result = previousValue + current;
        break;
      case '-':
        result = previousValue - current;
        break;
      case '*':
        result = previousValue * current;
        break;
      case '/':
        result = current !== 0 ? previousValue / current : 0;
        break;
    }
    
    setDisplay(result.toString());
    setCurrentInput(result.toString());
    setExpression(result.toString());
    setPreviousValue(result);
  };

  const handleClear = () => {
    setDisplay('0');
    setCurrentInput('');
    setPreviousValue(0);
    setOperation('');
    setExpression('');
  };

  return (
    <div className="max-w-md mx-auto">
      <h2 className="text-xl sm:text-2xl font-bold mb-3 sm:mb-4 text-dark-dark text-center">Calculator</h2>
      <div className="bg-white border border-dark-med/30 rounded-lg p-4 sm:p-6 shadow-md">
        <div className="bg-light-back border border-dark-med/20 rounded-lg p-3 sm:p-4 mb-3 sm:mb-4 text-right text-lg sm:text-2xl font-mono text-dark-dark min-h-12 sm:min-h-15 flex items-center justify-end break-all">
          {display}
        </div>
        <div className="grid grid-cols-4 gap-1.5 sm:gap-2">
          <button 
            onClick={handleClear} 
            className="col-span-2 bg-red-500 hover:bg-red-600 text-white font-bold py-3 sm:py-4 rounded-lg transition-colors text-sm sm:text-base"
          >
            C
          </button>
          <button 
            onClick={() => handleOperator('/')} 
            className="bg-mid-green hover:bg-dark-green text-white font-bold py-3 sm:py-4 rounded-lg transition-colors text-sm sm:text-base"
          >
            /
          </button>
          <button 
            onClick={() => handleOperator('*')} 
            className="bg-mid-green hover:bg-dark-green text-white font-bold py-3 sm:py-4 rounded-lg transition-colors text-sm sm:text-base"
          >
            *
          </button>
          
          <button 
            onClick={() => handleDigit('7')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-3 sm:py-4 rounded-lg transition-colors text-sm sm:text-base"
          >
            7
          </button>
          <button 
            onClick={() => handleDigit('8')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            8
          </button>
          <button 
            onClick={() => handleDigit('9')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            9
          </button>
          <button 
            onClick={() => handleOperator('-')} 
            className="bg-mid-green hover:bg-dark-green text-white font-bold py-4 rounded-lg transition-colors"
          >
            -
          </button>
          
          <button 
            onClick={() => handleDigit('4')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            4
          </button>
          <button 
            onClick={() => handleDigit('5')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            5
          </button>
          <button 
            onClick={() => handleDigit('6')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            6
          </button>
          <button 
            onClick={() => handleOperator('+')} 
            className="bg-mid-green hover:bg-dark-green text-white font-bold py-4 rounded-lg transition-colors"
          >
            +
          </button>
          
          <button 
            onClick={() => handleDigit('1')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            1
          </button>
          <button 
            onClick={() => handleDigit('2')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            2
          </button>
          <button 
            onClick={() => handleDigit('3')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            3
          </button>
          <button 
            onClick={handleEquals} 
            className="row-span-2 bg-dark-green hover:bg-dark-dark text-white font-bold rounded-lg transition-colors"
          >
            =
          </button>
          
          <button 
            onClick={() => handleDigit('0')} 
            className="col-span-2 bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            0
          </button>
          <button 
            onClick={() => handleDigit('.')} 
            className="bg-light-light hover:bg-light-med border border-dark-med/20 text-dark-dark font-bold py-4 rounded-lg transition-colors"
          >
            .
          </button>
        </div>
      </div>
    </div>
  );
};

const PomodoroView = () => {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const timerIntervalRef = useRef<number | null>(null);
  
  const [currentTimer, setCurrentTimer] = useState(1200);
  const [breakTime, setBreakTime] = useState(300);
  const [workTime, setWorkTime] = useState(1200);
  const [currentAction, setCurrentAction] = useState<'Working' | 'Break'>('Working');

  const percentageToRadians = (percentage: number): number => {
    const startAngle = (3 * Math.PI) / 2;
    const fullCircle = 2 * Math.PI;
    return startAngle + (fullCircle * (percentage / 100));
  };

  const renderFrame = useCallback(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.clearRect(0, 0, 300, 300);

    // Background
    ctx.fillStyle = '#FAF7F2';
    ctx.fillRect(0, 0, 300, 300);

    // Background circle
    ctx.fillStyle = 'white';
    ctx.beginPath();
    ctx.arc(150, 150, 123, 0, 2 * Math.PI);
    ctx.fill();

    // Progress arc
    ctx.fillStyle = currentAction === 'Working' ? 'rgba(113, 153, 132, 0.6)' : 'rgba(163, 193, 176, 0.6)';
    ctx.beginPath();
    ctx.moveTo(150, 150);
    ctx.lineTo(150, 37);
    const percentage = currentAction === 'Working' 
      ? (currentTimer / workTime) 
      : (currentTimer / breakTime);
    const endAngle = percentageToRadians(percentage * 100);
    ctx.arc(150, 150, 112, (3 * Math.PI) / 2, endAngle);
    ctx.lineTo(150, 150);
    ctx.fill();

    // Text
    ctx.font = 'bold 36px Inter';
    ctx.fillStyle = '#242424';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(toDuration(currentTimer), 150, 150);
  }, [currentTimer, currentAction, workTime, breakTime]);

  useEffect(() => {
    renderFrame();
  }, [renderFrame]);

  const startTimer = () => {
    if (timerIntervalRef.current === null) {
      timerIntervalRef.current = setInterval(() => {
        setCurrentTimer(prev => {
          const newTime = prev - 1;
          
          if (newTime <= 0) {
            if (currentAction === 'Working') {
              setCurrentAction('Break');
              window.alert('Stop working! Time for a break.');
              return breakTime;
            } else {
              setCurrentAction('Working');
              window.alert('Start working!');
              return workTime;
            }
          }
          
          return newTime;
        });
      }, 1000);
    }
  };

  const stopTimer = () => {
    if (timerIntervalRef.current !== null) {
      clearInterval(timerIntervalRef.current);
      timerIntervalRef.current = null;
    }
  };

  const resetTimer = () => {
    stopTimer();
    setCurrentTimer(workTime);
    setCurrentAction('Working');
  };

  const handleSetWorkTime = (n: number) => {
    setWorkTime(n);
    setTimeout(() => {
      resetTimer();
    }, 100);
  };

  const handleSetBreakTime = (n: number) => {
    setBreakTime(n);
    setTimeout(() => {
      resetTimer();
    }, 100);
  };

  return (
    <div className="max-w-2xl mx-auto">
      <h2 className="text-xl sm:text-2xl font-bold mb-3 sm:mb-4 text-dark-dark text-center">Pomodoro Timer</h2>
      
      <div className="flex flex-col items-center gap-3 sm:gap-4">
        <canvas
          ref={canvasRef}
          width={300}
          height={300}
          className="rounded-lg shadow-md max-w-full h-auto"
        />

        <div className="flex gap-2 sm:gap-3 flex-wrap justify-center">
          <button 
            onClick={startTimer} 
            className="px-4 sm:px-6 py-2 bg-mid-green hover:bg-dark-green text-white font-bold rounded-lg transition-colors text-sm sm:text-base"
          >
            Start
          </button>
          <button 
            onClick={stopTimer} 
            className="px-4 sm:px-6 py-2 bg-red-500 hover:bg-red-600 text-white font-bold rounded-lg transition-colors text-sm sm:text-base"
          >
            Stop
          </button>
          <button 
            onClick={resetTimer} 
            className="px-4 sm:px-6 py-2 bg-dark-med hover:bg-dark-dark text-white font-bold rounded-lg transition-colors text-sm sm:text-base"
          >
            Reset
          </button>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4 w-full mt-2">
          <div className="bg-white p-3 sm:p-4 rounded-lg border border-dark-med/20">
            <label className="block text-dark-dark font-semibold mb-2 text-sm sm:text-base">Work Time (minutes)</label>
            <select 
              onChange={(e) => handleSetWorkTime(parseInt(e.target.value))} 
              defaultValue={1200}
              className="w-full px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
            >
              <option value="600">10</option>
              <option value="900">15</option>
              <option value="1200">20</option>
              <option value="1500">25</option>
              <option value="1800">30</option>
            </select>
          </div>

          <div className="bg-white p-3 sm:p-4 rounded-lg border border-dark-med/20">
            <label className="block text-dark-dark font-semibold mb-2 text-sm sm:text-base">Break Time (minutes)</label>
            <select 
              onChange={(e) => handleSetBreakTime(parseInt(e.target.value))} 
              defaultValue={300}
              className="w-full px-3 sm:px-4 py-2 border border-dark-med/30 rounded-lg bg-white text-dark-dark focus:outline-none focus:ring-2 focus:ring-mid-green text-sm sm:text-base"
            >
              <option value="60">1</option>
              <option value="180">3</option>
              <option value="300">5</option>
              <option value="600">10</option>
            </select>
          </div>
        </div>
      </div>
    </div>
  );
};

function toDuration(seconds: number): string {
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;
  return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}

const CalendarView = () => {
  const [value, setValue] = useState<Date>(new Date());

  return (
    <div className="max-w-3xl mx-auto">
      <h2 className="text-xl sm:text-2xl font-bold mb-4 sm:mb-6 text-dark-dark text-center">Calendar</h2>
      <div className="calendar-container flex justify-center overflow-x-auto">
        <Calendar
          onChange={(value) => setValue(value as Date)}
          value={value}
          className="border border-dark-med/30 rounded-lg shadow-md bg-white"
        />
      </div>
      <div className="mt-4 sm:mt-6 p-3 sm:p-4 bg-white rounded-lg border border-dark-med/20 text-center">
        <p className="text-dark-med text-xs sm:text-sm">Selected Date:</p>
        <p className="text-dark-dark font-bold text-sm sm:text-lg">
          {value.toLocaleDateString('en-US', { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric' 
          })}
        </p>
      </div>
    </div>
  );
}
