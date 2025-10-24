USE Prog_A25_BD_Projet;
GO
CREATE TRIGGER trg_VerifierPlaceStationnement 
ON stationnementEntreeSortie 
AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @nbVoituresActuelles INT;
	DECLARE @capaciteMax INT;
	DECLARE @reponse NVARCHAR(100);
	DECLARE @IdStationnement INT;
	DECLARE @IdUtilisateur INT;
	
	SELECT TOP 1 @capaciteMax = nombrePlaceMax FROM stationnement WHERE numStationnement = @IdStationnement;

	SELECT @nbVoituresActuelles = COUNT(*)
    FROM stationnementEntreeSortie
    WHERE dateSortie IS NULL OR dateSortie > dateEntree;

	IF @nbVoituresActuelles >= @capaciteMax
	BEGIN
		ROLLBACK TRANSACTION;
		RETURN;
	END

	SELECT @IdUtilisateur = i.idUtilisateur
    FROM inserted i;

    IF EXISTS (
        SELECT 1 FROM reservation r
        WHERE r.idUtilisateur = @idUtilisateur
          AND r.dateDebut <= GETDATE()
          AND r.dateFin >= GETDATE()
    )
    BEGIN
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
