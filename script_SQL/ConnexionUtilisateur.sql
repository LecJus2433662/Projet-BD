CREATE PROCEDURE Connexionutilisateur
	@Email NVARCHAR(100),
	@MotDePasse NVARCHAR(100)
AS
	BEGIN
		DECLARE @Id int;
		SELECT @Id = noUtilisateur
		FROM utilisateur
		WHERE @Email = email
		AND motDePasse = HASHBYTES('SHA2_256', @MotDePasse);

		IF @Id IS NULL
			SELECT -1 AS Resultat;
		ELSE
			SELECT @Id AS Resultat;
	END
Go
