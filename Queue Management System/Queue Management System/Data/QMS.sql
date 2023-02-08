PGDMP             
            {            QMS    15.1    15.1                0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false                       0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false                       0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false                       1262    16460    QMS    DATABASE     x   CREATE DATABASE "QMS" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_Kenya.1252';
    DROP DATABASE "QMS";
                postgres    false            �            1259    16490    queue    TABLE     �   CREATE TABLE public.queue (
    id integer NOT NULL,
    servicepointid integer NOT NULL,
    status integer,
    createdat timestamp without time zone DEFAULT now(),
    updatedat time without time zone,
    completedat time without time zone
);
    DROP TABLE public.queue;
       public         heap    postgres    false            �            1259    16489    quelist_id_seq    SEQUENCE     �   CREATE SEQUENCE public.quelist_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE public.quelist_id_seq;
       public          postgres    false    219                       0    0    quelist_id_seq    SEQUENCE OWNED BY     ?   ALTER SEQUENCE public.quelist_id_seq OWNED BY public.queue.id;
          public          postgres    false    218            �            1259    16476    servicepoints    TABLE        CREATE TABLE public.servicepoints (
    id integer NOT NULL,
    name character varying(128),
    serviceproviderid integer
);
 !   DROP TABLE public.servicepoints;
       public         heap    postgres    false            �            1259    16475    servicepoints_id_seq    SEQUENCE     �   CREATE SEQUENCE public.servicepoints_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 +   DROP SEQUENCE public.servicepoints_id_seq;
       public          postgres    false    217                       0    0    servicepoints_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public.servicepoints_id_seq OWNED BY public.servicepoints.id;
          public          postgres    false    216            �            1259    16467    users    TABLE     �   CREATE TABLE public.users (
    id integer NOT NULL,
    name character varying(128) NOT NULL,
    password text NOT NULL,
    role character varying(128) NOT NULL,
    servicepointid integer
);
    DROP TABLE public.users;
       public         heap    postgres    false            �            1259    16466    users_id_seq    SEQUENCE     �   CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.users_id_seq;
       public          postgres    false    215                       0    0    users_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;
          public          postgres    false    214            q           2604    16493    queue id    DEFAULT     f   ALTER TABLE ONLY public.queue ALTER COLUMN id SET DEFAULT nextval('public.quelist_id_seq'::regclass);
 7   ALTER TABLE public.queue ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    218    219    219            p           2604    16479    servicepoints id    DEFAULT     t   ALTER TABLE ONLY public.servicepoints ALTER COLUMN id SET DEFAULT nextval('public.servicepoints_id_seq'::regclass);
 ?   ALTER TABLE public.servicepoints ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    217    216    217            o           2604    16470    users id    DEFAULT     d   ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);
 7   ALTER TABLE public.users ALTER COLUMN id DROP DEFAULT;
       public          postgres    false    215    214    215                      0    16490    queue 
   TABLE DATA           ^   COPY public.queue (id, servicepointid, status, createdat, updatedat, completedat) FROM stdin;
    public          postgres    false    219   c!                 0    16476    servicepoints 
   TABLE DATA           D   COPY public.servicepoints (id, name, serviceproviderid) FROM stdin;
    public          postgres    false    217   r$                 0    16467    users 
   TABLE DATA           I   COPY public.users (id, name, password, role, servicepointid) FROM stdin;
    public          postgres    false    215   �$                  0    0    quelist_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public.quelist_id_seq', 111, true);
          public          postgres    false    218                       0    0    servicepoints_id_seq    SEQUENCE SET     C   SELECT pg_catalog.setval('public.servicepoints_id_seq', 20, true);
          public          postgres    false    216                       0    0    users_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.users_id_seq', 22, true);
          public          postgres    false    214            |           2606    16498    queue quelist_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.queue
    ADD CONSTRAINT quelist_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.queue DROP CONSTRAINT quelist_pkey;
       public            postgres    false    219            x           2606    16483 $   servicepoints servicepoints_name_key 
   CONSTRAINT     _   ALTER TABLE ONLY public.servicepoints
    ADD CONSTRAINT servicepoints_name_key UNIQUE (name);
 N   ALTER TABLE ONLY public.servicepoints DROP CONSTRAINT servicepoints_name_key;
       public            postgres    false    217            z           2606    16481     servicepoints servicepoints_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public.servicepoints
    ADD CONSTRAINT servicepoints_pkey PRIMARY KEY (id);
 J   ALTER TABLE ONLY public.servicepoints DROP CONSTRAINT servicepoints_pkey;
       public            postgres    false    217            t           2606    16510    users unique_name 
   CONSTRAINT     L   ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_name UNIQUE (name);
 ;   ALTER TABLE ONLY public.users DROP CONSTRAINT unique_name;
       public            postgres    false    215            v           2606    16472    users users_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public            postgres    false    215            }           2606    24701    users fk_servicepointid    FK CONSTRAINT     �   ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_servicepointid FOREIGN KEY (servicepointid) REFERENCES public.servicepoints(id);
 A   ALTER TABLE ONLY public.users DROP CONSTRAINT fk_servicepointid;
       public          postgres    false    215    3194    217                       2606    16499 !   queue quelist_servicepointid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.queue
    ADD CONSTRAINT quelist_servicepointid_fkey FOREIGN KEY (servicepointid) REFERENCES public.servicepoints(id) ON DELETE CASCADE;
 K   ALTER TABLE ONLY public.queue DROP CONSTRAINT quelist_servicepointid_fkey;
       public          postgres    false    217    219    3194            ~           2606    16484 2   servicepoints servicepoints_serviceproviderid_fkey    FK CONSTRAINT     �   ALTER TABLE ONLY public.servicepoints
    ADD CONSTRAINT servicepoints_serviceproviderid_fkey FOREIGN KEY (serviceproviderid) REFERENCES public.users(id) ON DELETE CASCADE;
 \   ALTER TABLE ONLY public.servicepoints DROP CONSTRAINT servicepoints_serviceproviderid_fkey;
       public          postgres    false    215    217    3190               �  x�uU�q�@���H��}��WA���au�q,���Ă ���B���7�7��f�1IS���~k�b�y2(�;�<���#�L�ʋ}}�q��$˄|$��&���b�31za@������Wm�#�Ec�e��k��o� ��
��ňn� YY�!�Ó�e��⯑�my��P��t�	�ѥ�y�_ݓr �t'W�ڐ�>c��^��`@���QqU�(�>�Θ;t-���<<�P���gl2��;>H.x�����K�0~�$�
�C��FzH^#H*/#��	�������7J��F�$S&��w���b�d�|e�#�!l�Ƶ*U�b&�}�9w� ��K����R0�]e� ���P�����0�>V1v�0E<sct%d�mM��^g���X�c˛�� 5�X��;��d�.�p�ͰA1�A]%j��U��`�΋��p�v�U��v�l�Hz�;Z%��#&��u2f�m��;�M��aF�iu�3ؾ�t�y'(�%oåa��p4���a��7��H�άS��<���JX�N�3���\�R� �}��<�*���'1O�Q���yM11�Z�}c{�_�����7��ç�W�D��je��3ք���~������[b*�4���Z ��*�.�x�c��� ^=o���j�3�A�8�?��rB��^r{_�=] 7y���"��c	\X�ދ�u�E�3�	쏚C�_��J|s�	�=�d�g�����\N��g������S         V   x��;� ���aL������YQ\���)gĜ��T��,0d�hRBJQ.X�X���5k�#��f}�����ۢ�P0�>����         c   x�3�tL��̃��F�P�!��_iQq�!����e&�*�e���C䡪��+3�2��CUcWf�e
���6��̄�"Um�]�)W� _�D�     