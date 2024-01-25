PGDMP  7        
             |            Queue Management System    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    24576    Queue Management System    DATABASE     �   CREATE DATABASE "Queue Management System" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
 )   DROP DATABASE "Queue Management System";
                postgres    false                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
                pg_database_owner    false            �           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                   pg_database_owner    false    4            �            1259    24598    service_points    TABLE     p   CREATE TABLE public.service_points (
    id text NOT NULL,
    service_id text,
    service_provider_id text
);
 "   DROP TABLE public.service_points;
       public         heap    postgres    false    4            �            1259    24588    service_providers    TABLE     �   CREATE TABLE public.service_providers (
    id text NOT NULL,
    names text DEFAULT 'No Name'::text,
    email text NOT NULL,
    password text NOT NULL,
    is_administrator boolean DEFAULT false NOT NULL
);
 %   DROP TABLE public.service_providers;
       public         heap    postgres    false    4            �            1259    24580    services    TABLE     l   CREATE TABLE public.services (
    id text NOT NULL,
    description text DEFAULT 'No Description'::text
);
    DROP TABLE public.services;
       public         heap    postgres    false    4            �            1259    24616    tickets    TABLE     j  CREATE TABLE public.tickets (
    transaction_id integer NOT NULL,
    ticket_number text NOT NULL,
    service_id text NOT NULL,
    service_point_id text,
    time_printed timestamp without time zone NOT NULL,
    showed_up boolean,
    time_showed_up timestamp without time zone,
    time_finished timestamp without time zone,
    service_provider_id text
);
    DROP TABLE public.tickets;
       public         heap    postgres    false    4            �            1259    24615    tickets_transaction_id_seq    SEQUENCE     �   CREATE SEQUENCE public.tickets_transaction_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 1   DROP SEQUENCE public.tickets_transaction_id_seq;
       public          postgres    false    4    219            �           0    0    tickets_transaction_id_seq    SEQUENCE OWNED BY     Y   ALTER SEQUENCE public.tickets_transaction_id_seq OWNED BY public.tickets.transaction_id;
          public          postgres    false    218            )           2604    24619    tickets transaction_id    DEFAULT     �   ALTER TABLE ONLY public.tickets ALTER COLUMN transaction_id SET DEFAULT nextval('public.tickets_transaction_id_seq'::regclass);
 E   ALTER TABLE public.tickets ALTER COLUMN transaction_id DROP DEFAULT;
       public          postgres    false    219    218    219            �          0    24598    service_points 
   TABLE DATA           M   COPY public.service_points (id, service_id, service_provider_id) FROM stdin;
    public          postgres    false    217   L       �          0    24588    service_providers 
   TABLE DATA           Y   COPY public.service_providers (id, names, email, password, is_administrator) FROM stdin;
    public          postgres    false    216   �       �          0    24580    services 
   TABLE DATA           3   COPY public.services (id, description) FROM stdin;
    public          postgres    false    215           �          0    24616    tickets 
   TABLE DATA           �   COPY public.tickets (transaction_id, ticket_number, service_id, service_point_id, time_printed, showed_up, time_showed_up, time_finished, service_provider_id) FROM stdin;
    public          postgres    false    219   \        �           0    0    tickets_transaction_id_seq    SEQUENCE SET     I   SELECT pg_catalog.setval('public.tickets_transaction_id_seq', 60, true);
          public          postgres    false    218            +           2606    24587    services Services_pkey 
   CONSTRAINT     V   ALTER TABLE ONLY public.services
    ADD CONSTRAINT "Services_pkey" PRIMARY KEY (id);
 B   ALTER TABLE ONLY public.services DROP CONSTRAINT "Services_pkey";
       public            postgres    false    215            1           2606    24604 "   service_points service_points_pkey 
   CONSTRAINT     `   ALTER TABLE ONLY public.service_points
    ADD CONSTRAINT service_points_pkey PRIMARY KEY (id);
 L   ALTER TABLE ONLY public.service_points DROP CONSTRAINT service_points_pkey;
       public            postgres    false    217            3           2606    24640 5   service_points service_points_service_provider_id_key 
   CONSTRAINT        ALTER TABLE ONLY public.service_points
    ADD CONSTRAINT service_points_service_provider_id_key UNIQUE (service_provider_id);
 _   ALTER TABLE ONLY public.service_points DROP CONSTRAINT service_points_service_provider_id_key;
       public            postgres    false    217            -           2606    24597 -   service_providers service_providers_email_key 
   CONSTRAINT     i   ALTER TABLE ONLY public.service_providers
    ADD CONSTRAINT service_providers_email_key UNIQUE (email);
 W   ALTER TABLE ONLY public.service_providers DROP CONSTRAINT service_providers_email_key;
       public            postgres    false    216            /           2606    24595 (   service_providers service_providers_pkey 
   CONSTRAINT     f   ALTER TABLE ONLY public.service_providers
    ADD CONSTRAINT service_providers_pkey PRIMARY KEY (id);
 R   ALTER TABLE ONLY public.service_providers DROP CONSTRAINT service_providers_pkey;
       public            postgres    false    216            5           2606    24623    tickets tickets_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_pkey PRIMARY KEY (transaction_id);
 >   ALTER TABLE ONLY public.tickets DROP CONSTRAINT tickets_pkey;
       public            postgres    false    219            6           2606    24605 -   service_points service_points_service_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.service_points
    ADD CONSTRAINT service_points_service_id_fkey FOREIGN KEY (service_id) REFERENCES public.services(id) ON UPDATE CASCADE ON DELETE SET NULL;
 W   ALTER TABLE ONLY public.service_points DROP CONSTRAINT service_points_service_id_fkey;
       public          postgres    false    215    217    4651            7           2606    24610 6   service_points service_points_service_provider_id_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.service_points
    ADD CONSTRAINT service_points_service_provider_id_fkey FOREIGN KEY (service_provider_id) REFERENCES public.service_providers(id) ON UPDATE CASCADE ON DELETE SET NULL;
 `   ALTER TABLE ONLY public.service_points DROP CONSTRAINT service_points_service_provider_id_fkey;
       public          postgres    false    217    4655    216            �   <   x�+N-*�LN�/���+�70�,�
�� ��U���"#�"#tE�Hj�!j��b���� �j%       �   [   x�+.�700����+I��鹉�9z�����%gW1H�!gJ"P�J!),H,..�/J�-��5�L�(�,��%�%p�`!��4�=... :�*�      �   I   x�+N-*�LN�700���+.-J�KNU��r�e�8�SsJ2|��SsS�J��2�tI-�/�D���qqq �"]      �   Y   x�3���32454�,N-*�LN�700���4202�50�52U0��2"C=s#Kc�,q�@01"`�����������!�1z\\\ D�     