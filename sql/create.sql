CREATE TABLE
  "user" (
    id int4 PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    email varchar(80) UNIQUE NOT NULL,
    google_id varchar(255) UNIQUE NULL,
    displayName varchar(80) NOT NULL, 
    profile_picture varchar(500) NULL,
    role int4 NOT NULL DEFAULT 0,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login timestamp NULL
  );

CREATE TABLE
  course (
    id int4 PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    account_email varchar(80) references "user" (email) NOT NULL,
    course_name varchar(80) NOT NULL
  );

CREATE TABLE
  assignment (
    id int4 PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    course_id int references course (id) NOT NULL,
    assignment_name varchar(80), 
    assignment_description varchar(300) NULL, 
    due_date timestamp NULL, 
    mark_started timestamp NULL, 
    mark_complete timestamp NULL
  );

CREATE TABLE
  todoitem (
    id int4 PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    account_email varchar(80) references "user" (email) NOT NULL,
    category_name varchar(80) NOT NULL,
    parent_id int references todoitem (id) NULL,
    assignment_id int references assignment (id) NULL,
    todo_name varchar(80) NOT NULL, 
    completion_date timestamp NULL
  );

CREATE TABLE
  messages(
    id int4 PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    sender_account_email varchar(80) references "user" (email) NOT NULL,
    receiver_account_email varchar(80) references "user" (email) NOT NULL,
    message_text text NOT NULL,
    time_sent timestamp NOT NULL
  );