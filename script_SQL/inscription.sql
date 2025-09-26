USE Prog_A25_BD_Projet
GO

CREATE PROCEDURE ajout_utilisateur(@noUtilisateur int,@nom varchar(30), @Prenom varchar(30),@ville varchar(30),@pays varchar(30),@email varchar(255),@motDePasseChiffre varchar(255),@reponse NVARCHAR(250) OUTPUT) AS
BEGIN 
SET NOCOUNT ON
DECLARE @sel UNIQUEIDENTIFIER = NEWID();
BEGIN TRY
	INSERT INTO utilisateur(nom,prenom,ville,pays,email,motDePasse,sel)
	VALUES(@nom,@Prenom,@ville,@pays,@email,HASHBYTES('SHA2_512',@motDePasseChiffre +CAST(@sel AS NVARCHAR(36))),@sel)
	SET @reponse = (SELECT noUtilisateur from utilisateur where motDePasse =HASHBYTES('SHA2_512',@motDePasseChiffre +CAST(@sel AS NVARCHAR(36))) );
END TRY
BEGIN CATCH
	SET @reponse = 'mauvais mot de passe ou adresse courriel';
	END CATCH

END

	
