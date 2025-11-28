USE Prog_A25_Bd_Projet;
GO
CREATE PROCEDURE ajout_admin(
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

			INSERT INTO utilisateur(nom, prenom, ville, pays, email, motDePasse, sel, IsAdmin)
			VALUES(@nom, @Prenom, @ville, @pays, @email, 
				   HASHBYTES('SHA2_512', @motDePasseChiffre + CAST(@sel AS NVARCHAR(36))), 
				   @sel, 1);

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


DECLARE @reponse int;

EXEC dbo.ajout_admin
    @nom = 'Leclerc',
    @prenom = 'Justin',
    @ville = 'chicoutimi',
    @pays = 'canada',
    @email = 'justin_admin@gmail.com',
    @motDePasseChiffre = 'admin_Justin',
    @reponse = @reponse OUTPUT;

EXEC dbo.ajout_admin
    @nom = 'Bilynets',
    @prenom = 'Oleksandr',
    @ville = 'Jonqueire ',
    @pays = 'canada',
    @email = 'alex_admin@gmail.com',
    @motDePasseChiffre = 'admin_Alex',
    @reponse = @reponse OUTPUT;
SELECT @reponse as N'@message de réponse';
select * from utilisateur;