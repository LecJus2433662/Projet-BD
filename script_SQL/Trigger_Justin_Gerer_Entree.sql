GO
CREATE PROCEDURE sp_GererEntreeVehicule
(
    @numVehicule INT,
    @numBarriere INT,
    @numUtilisateur INT,
    @numStationnement INT,
    @idEntreeSortie INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Vérifier que le stationnement existe
        IF NOT EXISTS(SELECT 1 FROM stationnement WHERE numStationnement = @numStationnement)
            THROW 50001, 'Le stationnement n’existe pas.', 1;

        -- Vérifier que la barrière existe
        IF NOT EXISTS(SELECT 1 FROM barriere WHERE numBarriere = @numBarriere)
            THROW 50002, 'La barrière est invalide.', 1;

        -- Vérifier que le véhicule existe
        IF NOT EXISTS(SELECT 1 FROM vehicule WHERE numVehicule = @numVehicule)
            THROW 50003, 'Le véhicule est invalide.', 1;

        DECLARE @placesMax INT, @nbActuels INT;

        SELECT @placesMax = nombrePlaceMax
        FROM stationnement
        WHERE numStationnement = @numStationnement;

        SELECT @nbActuels = COUNT(*)
        FROM stationnementEntreeSortie
        WHERE numStationnement = @numStationnement
          AND dateSortie IS NULL;

        -- Vérifier si plein
        IF (@nbActuels >= @placesMax)
        BEGIN
            UPDATE stationnement SET estPlein = 1 WHERE numStationnement = @numStationnement;
            THROW 50004, 'Stationnement complet.', 1;
        END

        -- Insérer l’entrée
        INSERT INTO stationnementEntreeSortie(
            dateEntree, dateSortie, paiementSortie, paiementRecu,
            numVehicule, numBarriere, numUtilisateur, numStationnement, reservation
        )
        VALUES (
            GETDATE(), NULL, 0, 0,
            @numVehicule, @numBarriere, @numUtilisateur, @numStationnement, 0
        );

        SET @idEntreeSortie = SCOPE_IDENTITY();

        -- Vérifier si on devient plein après l’entrée
        SET @nbActuels += 1;
        IF (@nbActuels >= @placesMax)
            UPDATE stationnement SET estPlein = 1 WHERE numStationnement = @numStationnement;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
        THROW 50010, @msg, 1;
    END CATCH
END
GO
