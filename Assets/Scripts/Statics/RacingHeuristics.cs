public static class RacingHeuristics
{
    public struct DadosRecurso
    {
        public bool semRecurso;
        public bool recurso5Menos;
        public bool recurso20Menos;
        public bool recurso50Menos;
        public bool recurso75Mais;
        public bool recurso90Mais;

        public DadosRecurso(float pneu, float comb)
        {
            semRecurso = pneu < 1f || comb < 1f;
            recurso5Menos = pneu < 5f || comb < 5f;
            recurso20Menos = pneu < 20f || comb < 20f;
            recurso50Menos = pneu < 50f || comb < 50f;
            recurso75Mais = pneu > 75f && comb > 75f;
            recurso90Mais = pneu > 90f && comb > 90f;
        }
    }

    public struct DadosDistancias
    {
        public bool ameacaAtras10m;
        public bool ameacaAtras20m;
        public bool ameacaAtras40m;

        public bool oportunidade10m;
        public bool oportunidade20m;
        public bool oportunidade40m;
        public bool oportunidade60m;

        public DadosDistancias(float distTras, float distFrente)
        {
            ameacaAtras10m = distTras < 10f;
            ameacaAtras20m = distTras < 20f;
            ameacaAtras40m = distTras < 40f;

            oportunidade10m = distFrente < 10f;
            oportunidade20m = distFrente < 20f;
            oportunidade40m = distFrente < 40f;
            oportunidade60m = distFrente < 60f;
        }
    }
}