import { useState } from 'react';

type Tool = 'Notes' | 'Calculator' | 'Pomodoro' | 'Calendar';

export const Tools = () => {
  const [activeTool, setActiveTool] = useState<Tool>('Notes');

  return (
    <div className="tools">
      <h1>Tools</h1>
      
      <div className="tool-selector">
        <button
          className={activeTool === 'Notes' ? 'active' : ''}
          onClick={() => setActiveTool('Notes')}
        >
          Notes
        </button>
        <button
          className={activeTool === 'Calculator' ? 'active' : ''}
          onClick={() => setActiveTool('Calculator')}
        >
          Calculator
        </button>
        <button
          className={activeTool === 'Pomodoro' ? 'active' : ''}
          onClick={() => setActiveTool('Pomodoro')}
        >
          Pomodoro
        </button>
        <button
          className={activeTool === 'Calendar' ? 'active' : ''}
          onClick={() => setActiveTool('Calendar')}
        >
          Calendar
        </button>
      </div>

      <div className="tool-content">
        {activeTool === 'Notes' && <NotesView />}
        {activeTool === 'Calculator' && <CalculatorView />}
        {activeTool === 'Pomodoro' && <PomodoroView />}
        {activeTool === 'Calendar' && <CalendarView />}
      </div>
    </div>
  );
};

const NotesView = () => {
  const [content, setContent] = useState(
    localStorage.getItem('consilium_notes') || ''
  );

  const handleChange = (value: string) => {
    setContent(value);
    localStorage.setItem('consilium_notes', value);
  };

  return (
    <div className="notes-view">
      <textarea
        value={content}
        onChange={(e) => handleChange(e.target.value)}
        placeholder="Start typing your notes..."
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
    <div className="calculator-view">
      <div className="calc-display">{display}</div>
      <div className="calc-buttons">
        <button onClick={handleClear}>C</button>
        <button onClick={() => handleOperator('/')}>/</button>
        <button onClick={() => handleOperator('*')}>*</button>
        <button onClick={() => handleOperator('-')}>-</button>
        <button onClick={() => handleDigit('7')}>7</button>
        <button onClick={() => handleDigit('8')}>8</button>
        <button onClick={() => handleDigit('9')}>9</button>
        <button onClick={() => handleOperator('+')}>+</button>
        <button onClick={() => handleDigit('4')}>4</button>
        <button onClick={() => handleDigit('5')}>5</button>
        <button onClick={() => handleDigit('6')}>6</button>
        <button onClick={() => handleDigit('1')}>1</button>
        <button onClick={() => handleDigit('2')}>2</button>
        <button onClick={() => handleDigit('3')}>3</button>
        <button onClick={() => handleDigit('0')} className="span-2">0</button>
        <button onClick={() => handleDigit('.')}>.</button>
        <button onClick={handleEquals} className="equals">=</button>
      </div>
    </div>
  );
};

const PomodoroView = () => {
  const [workTime] = useState(25);
  const [currentAction, setCurrentAction] = useState<'Working' | 'Break'>('Working');
  const [timeLeft, setTimeLeft] = useState(workTime * 60);
  const [isRunning, setIsRunning] = useState(false);

  return (
    <div className="pomodoro-view">
      <div className="pomodoro-display">
        <h2>{currentAction}</h2>
        <div className="timer">
          {Math.floor(timeLeft / 60)}:{String(timeLeft % 60).padStart(2, '0')}
        </div>
      </div>
      <div className="pomodoro-controls">
        <button onClick={() => setIsRunning(!isRunning)} className="btn-primary">
          {isRunning ? 'Pause' : 'Start'}
        </button>
        <button onClick={() => {
          setIsRunning(false);
          setTimeLeft(workTime * 60);
          setCurrentAction('Working');
        }}>
          Reset
        </button>
      </div>
    </div>
  );
};

const CalendarView = () => {
  const today = new Date();
  
  return (
    <div className="calendar-view">
      <h2>{today.toLocaleDateString('en-US', { month: 'long', year: 'numeric' })}</h2>
      <p>Calendar view - Feature coming soon</p>
    </div>
  );
};
