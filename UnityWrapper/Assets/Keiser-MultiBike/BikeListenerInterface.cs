namespace KeiserSDK
{
    public interface BikeListenerInterface
    {
        void gotNewBike (KeiserBike bike);
        void lostBike (KeiserBike bike);
        void updatedBike (KeiserBike bike);
    }
}