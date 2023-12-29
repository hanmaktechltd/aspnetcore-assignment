-- Table: public.admins

-- DROP TABLE IF EXISTS public.admins;

CREATE TABLE IF NOT EXISTS public.admins
(
    adminid integer NOT NULL DEFAULT nextval('admins_adminid_seq'::regclass),
    username character varying(50) COLLATE pg_catalog."default" NOT NULL,
    password character varying(100) COLLATE pg_catalog."default" NOT NULL,
    email character varying(100) COLLATE pg_catalog."default" NOT NULL,
    phonenumber character varying(20) COLLATE pg_catalog."default",
    createdat timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT admins_pkey PRIMARY KEY (adminid),
    CONSTRAINT admins_email_key UNIQUE (email),
    CONSTRAINT admins_username_key UNIQUE (username)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.admins
    OWNER to postgres;
	
	
	-- Table: public.finishedtable

-- DROP TABLE IF EXISTS public.finishedtable;

CREATE TABLE IF NOT EXISTS public.finishedtable
(
    finishedid integer NOT NULL DEFAULT nextval('finishedtable_finishedid_seq'::regclass),
    ticketnumber character varying(20) COLLATE pg_catalog."default" NOT NULL,
    servicepointid integer NOT NULL,
    markedtime timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT finishedtable_pkey PRIMARY KEY (finishedid)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.finishedtable
    OWNER to postgres;
	
	
	-- Table: public.noshowtable

-- DROP TABLE IF EXISTS public.noshowtable;

CREATE TABLE IF NOT EXISTS public.noshowtable
(
    noshowid integer NOT NULL DEFAULT nextval('noshowtable_noshowid_seq'::regclass),
    ticketnumber character varying(20) COLLATE pg_catalog."default" NOT NULL,
    servicepointid integer NOT NULL,
    markedtime timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT noshowtable_pkey PRIMARY KEY (noshowid)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.noshowtable
    OWNER to postgres;
	
	
	-- Table: public.queueentry

-- DROP TABLE IF EXISTS public.queueentry;

CREATE TABLE IF NOT EXISTS public.queueentry
(
    id integer NOT NULL DEFAULT nextval('queueentry_id_seq'::regclass),
    ticketnumber character varying(50) COLLATE pg_catalog."default" NOT NULL,
    servicepoint character varying(100) COLLATE pg_catalog."default" NOT NULL,
    customername character varying(100) COLLATE pg_catalog."default" NOT NULL,
    checkintime timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    servicepointid integer,
    recallcount integer DEFAULT 0,
    noshow integer DEFAULT 0,
    markfinished integer DEFAULT 0,
    CONSTRAINT queueentry_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.queueentry
    OWNER to postgres;
	
	
	-- Table: public.serviceproviders

-- DROP TABLE IF EXISTS public.serviceproviders;

CREATE TABLE IF NOT EXISTS public.serviceproviders
(
    id integer NOT NULL DEFAULT nextval('serviceproviders_id_seq'::regclass),
    username character varying(50) COLLATE pg_catalog."default" NOT NULL,
    email character varying(100) COLLATE pg_catalog."default" NOT NULL,
    phone character varying(20) COLLATE pg_catalog."default" NOT NULL,
    passwordhash character varying(255) COLLATE pg_catalog."default" NOT NULL,
    registrationdate timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    servicepoint character varying(100) COLLATE pg_catalog."default",
    servicetypeid integer,
    isauthorized boolean DEFAULT false,
    CONSTRAINT serviceproviders_pkey PRIMARY KEY (id),
    CONSTRAINT fk_servicetypeid FOREIGN KEY (servicetypeid)
        REFERENCES public.servicetypes (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.serviceproviders
    OWNER to postgres;
	
	
	-- Table: public.servicetypes

-- DROP TABLE IF EXISTS public.servicetypes;

CREATE TABLE IF NOT EXISTS public.servicetypes
(
    id integer NOT NULL DEFAULT nextval('servicetypes_id_seq'::regclass),
    name character varying(100) COLLATE pg_catalog."default" NOT NULL,
    description text COLLATE pg_catalog."default",
    CONSTRAINT servicetypes_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.servicetypes
    OWNER to postgres;
	
	