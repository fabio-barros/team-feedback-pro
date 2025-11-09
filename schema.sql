-- WARNING: This schema is for context only and is not meant to be run.
-- Table order and constraints may not be valid for execution.

CREATE TABLE public.feedbacks (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  author_id bigint NOT NULL,
  target_id bigint NOT NULL,
  manager_id bigint,
  team_id bigint,
  title character varying NOT NULL,
  message text NOT NULL,
  created_at timestamp without time zone NOT NULL DEFAULT now(),
  updated_at timestamp without time zone NOT NULL DEFAULT now(),
  CONSTRAINT fk_feedback_author FOREIGN KEY (author_id) REFERENCES public.users(id),
  CONSTRAINT fk_feedback_target FOREIGN KEY (target_id) REFERENCES public.users(id),
  CONSTRAINT fk_feedback_manager FOREIGN KEY (manager_id) REFERENCES public.users(id),
  CONSTRAINT fk_feedback_team FOREIGN KEY (team_id) REFERENCES public.teams(id)
);
CREATE TABLE public.teams (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  name character varying NOT NULL,
  manager_id bigint,
  created_at timestamp without time zone NOT NULL DEFAULT now(),
  updated_at timestamp without time zone NOT NULL DEFAULT now(),
  CONSTRAINT teams_pkey PRIMARY KEY (id),
  CONSTRAINT TEAMS_manager_id_fkey FOREIGN KEY (manager_id) REFERENCES public.users(id)
);
CREATE TABLE public.users (
  id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
  name character varying NOT NULL,
  email character varying NOT NULL UNIQUE,
  password_hash text NOT NULL,
  role character varying NOT NULL,
  team_id bigint,
  created_at timestamp with time zone NOT NULL DEFAULT now(),
  updated_at timestamp without time zone NOT NULL DEFAULT now(),
  CONSTRAINT users_pkey PRIMARY KEY (id),
  CONSTRAINT USERS_team_id_fkey FOREIGN KEY (team_id) REFERENCES public.teams(id)
);