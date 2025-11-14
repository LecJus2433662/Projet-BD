USE Prog_A25_Bd_Projet_Prog
GO

CREATE PROCEDURE ajout_utilisateur(
    @noUtilisateur int,
    @nom varchar(30),
    @Prenom varchar(30),
    @ville varchar(30),
    @pays varchar(30),
    @email varchar(255),
    @motDePasseChiffre varchar(255),
    @reponse int OUTPUT
) 
AS
BEGIN 
    SET NOCOUNT ON;
    DECLARE @sel UNIQUEIDENTIFIER = NEWID();
    BEGIN TRY

	if NOT EXISTS(select email from utilisateur where email = @email)
		BEGIN

			INSERT INTO utilisateur(nom, prenom, ville, pays, email, motDePasse, sel)
			VALUES(@nom, @Prenom, @ville, @pays, @email, 
				   HASHBYTES('SHA2_512', @motDePasseChiffre + CAST(@sel AS NVARCHAR(36))), 
				   @sel);

			SET @reponse = SCOPE_IDENTITY()
		END
	ELSE
		BEGIN
			set @reponse = -2;
		END

    END TRY
    BEGIN CATCH
        SET @reponse = -1;
    END CATCH
END
GO

-- Appel de la procédure
DECLARE @reponse int;

EXEC dbo.ajout_utilisateur
    @noUtilisateur = 1,
    @nom = 'benoit',
    @prenom = 'benoit',
    @ville = 'chicoutimi',
    @pays = 'canada',
    @email = 'benoit@benoit.ca',
    @motDePasseChiffre = 'secret',
    @reponse = @reponse OUTPUT;

SELECT @reponse as N'@message de réponse';
select * from utilisateur;