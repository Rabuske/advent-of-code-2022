using System.Numerics;

record Point2D <T> where T : INumber<T> {
    public T x {get; init;}
    public T y {get; init;}

    public Point2D(T x, T y) {
        this.x = x;
        this.y = y;
    }

    public Point2D(string x, string y) {
        this.x = T.Parse(x, null);
        this.y = T.Parse(y, null);
    }    

    public static Point2D<T> operator -(Point2D<T> p1, Point2D<T> p2) => new Point2D<T>(p1.x - p2.x, p1.y - p2.y);
    public static Point2D<T> operator +(Point2D<T> p1, Point2D<T> p2) => new Point2D<T>(p1.x + p2.x, p1.y + p2.y);
    public static Point2D<T> operator *(Point2D<T> p1, Point2D<T> p2) => new Point2D<T>(p1.x * p2.x, p1.y * p2.y);
    public static Point2D<T> operator *(Point2D<T> p1, T m) => new Point2D<T>(p1.x * m, p1.y * m);

    public List<Point2D<T>> GenerateAdjacent(bool includeDiagonal = false, bool includePoint = false) {
        var result = new List<Point2D<T>>();

        if(includeDiagonal) result.Add(new Point2D<T>(-T.One,-T.One));
        result.Add(new (-T.One, T.Zero));
        if(includeDiagonal) result.Add(new (-T.One, T.One));
        
        result.Add(new (T.Zero, -T.One));
        if(includePoint) result.Add(this);
        result.Add(new (T.Zero, -T.One));

        if(includeDiagonal) result.Add(new (T.One, -T.One));
        result.Add(new (T.One, T.Zero));
        if(includeDiagonal) result.Add(new (T.One, T.One));

        return result.Select(r => this + r).ToList();
    }

    public T ManhattanDistance(Point2D<T> p) => T.Abs(this.x - p.x) + T.Abs(this.y - p.y);
    public T CrossProduct(Point2D<T> p) => this.x * p.y - this.y * p.x;
}