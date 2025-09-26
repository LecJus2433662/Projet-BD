USE master;
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Prog_A25_BD_Projet')
BEGIN
	ALTER DATABASE Prog_A25_BD_Projet SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE Prog_A25_BD_Projet;
END

CREATE DATABASE Prog_A25_BD_Projet;
GO
USE Prog_A25_BD_Projet;

CREATE TABLE utilisateur 
(
	noUtilisateur	INT				NOT NULL		IDENTITY(1,1),
	nom				VARCHAR(30)		NOT NULL,
	prenom			VARCHAR(30)		NOT NULL,
	ville			VARCHAR(30)		NOT NULL,
	pays			VARCHAR(30)		NOT NULL,
	email			VARCHAR(255)	NOT NULL,
	motDePasse		BINARY(64)	NOT NULL,
	sel				UNIQUEIDENTIFIER NOT NULL,
	PRIMARY KEY (noUtilisateur)
);

CREATE TABLE vehicule 
(
	numVehicule						INT				NOT NULL		IDENTITY(1,1),
	marque							VARCHAR(30)		NOT NULL,
	modele							VARCHAR(30)		NOT NULL,
	plaqueImmatriculation			VARCHAR(30)		NOT NULL,
	couleur							VARCHAR(30)		NOT NULL,
	PRIMARY KEY (numVehicule)
);

CREATE TABLE capteur 
(
	numCapteur		INT				NOT NULL		IDENTITY(1,1),
	mouvement		Decimal(4,2)	NOT NULL,
	date			date			NOT NULL,
	PRIMARY KEY (numCapteur)
);

CREATE TABLE stationnement 
(
	numStationnement			INT				NOT NULL		IDENTITY(1,1),
	nombrePlaceMax				INT				NOT NULL,
	dureeMaxStationnement		time			NOT NULL,
	entreSortieStationnement	INT				NOT NULL,
	tarif 						DECIMAL(4,2)	NOT NULL,
	estPlein					BIT,
	PRIMARY KEY (numStationnement)
);

CREATE TABLE barriere 
(
	numBarriere				INT				NOT NULL		IDENTITY(1,1),
	dureeAttente			INT				NOT NULL,
	noBarriereOuverture		INT				NOT NULL,
	tempsOuverture			TIME			NOT NULL,
	numeroCapteur			INT				NOT NULL,
	PRIMARY KEY (numBarriere)
);

CREATE TABLE stationnementEntreeSortie 
(
	entreSortieStationnement		INT				NOT NULL		IDENTITY(1,1),
	dateEntree						date 			NOT NULL,
	dateSortie						date			NOT NULL,
	paiementSortie					DECIMAL(4,2)	NOT NULL,
	paiementRecu					BIT				NOT NULL,
	numVehicule						INT 			NOT NULL,
	numBarriere						INT 			NOT NULL,
	numUtilisateur					INT 			NOT NULL,
	reservation						BIT,
	PRIMARY KEY (entreSortieStationnement)
);	

