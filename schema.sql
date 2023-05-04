--
-- PostgreSQL database dump
--

-- Dumped from database version 14.6 (Debian 14.6-1.pgdg110+1)
-- Dumped by pg_dump version 14.6 (Debian 14.6-1.pgdg110+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: locations; Type: TABLE; Schema: public; Owner: dboperator
--

CREATE TABLE public.locations (
    id integer NOT NULL,
    address text NOT NULL,
    web character varying(255) NOT NULL,
    name character varying(100) NOT NULL
);


ALTER TABLE public.locations OWNER TO dboperator;

--
-- Name: Location_id_seq; Type: SEQUENCE; Schema: public; Owner: dboperator
--

CREATE SEQUENCE public."Location_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Location_id_seq" OWNER TO dboperator;

--
-- Name: Location_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: dboperator
--

ALTER SEQUENCE public."Location_id_seq" OWNED BY public.locations.id;


--
-- Name: measurements; Type: TABLE; Schema: public; Owner: dboperator
--

CREATE TABLE public.measurements (
    location_id integer NOT NULL,
    "timestamp" timestamp with time zone NOT NULL,
    value double precision NOT NULL,
    unit character varying(10) NOT NULL
);


ALTER TABLE public.measurements OWNER TO dboperator;

--
-- Name: locations id; Type: DEFAULT; Schema: public; Owner: dboperator
--

ALTER TABLE ONLY public.locations ALTER COLUMN id SET DEFAULT nextval('public."Location_id_seq"'::regclass);


--
-- Data for Name: locations; Type: TABLE DATA; Schema: public; Owner: dboperator
--

COPY public.locations (id, address, web, name) FROM stdin;
1	Postgasshalde 50, 3011 Bern	http://192.168.230.136:1081/	Rathaus-Parking Amag Bern
2	Talstation Schönegg, Schönegg 31, 6300 Zug	http://localhost:5290/	Zugerbergbahn AG
\.


--
-- Data for Name: measurements; Type: TABLE DATA; Schema: public; Owner: dboperator
--

COPY public.measurements (location_id, "timestamp", value, unit) FROM stdin;
1	2023-02-05 01:06:56+00	128.87	A
1	2023-02-05 01:06:51+00	121.76	VA
\.


--
-- Name: Location_id_seq; Type: SEQUENCE SET; Schema: public; Owner: dboperator
--

SELECT pg_catalog.setval('public."Location_id_seq"', 2, true);


--
-- Name: locations Location_pkey; Type: CONSTRAINT; Schema: public; Owner: dboperator
--

ALTER TABLE ONLY public.locations
    ADD CONSTRAINT "Location_pkey" PRIMARY KEY (id);


--
-- Name: fki_location_id; Type: INDEX; Schema: public; Owner: dboperator
--

CREATE INDEX fki_location_id ON public.measurements USING btree (location_id);


--
-- Name: measurements location_id; Type: FK CONSTRAINT; Schema: public; Owner: dboperator
--

ALTER TABLE ONLY public.measurements
    ADD CONSTRAINT location_id FOREIGN KEY (location_id) REFERENCES public.locations(id) NOT VALID;


--
-- PostgreSQL database dump complete
--

