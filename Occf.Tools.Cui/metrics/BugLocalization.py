def CalculateMetric(executedAndPassedCount, passedCount, executedAndFailedCount, failedCount):
	
	if passedCount == 0 and failedCount == 0:
		return [float("nan"), float("nan"), float("nan")]
	if passedCount == 0:
		return [1.0, float("nan"), executedAndFailedCount / failedCount]
	if failedCount == 0:
		return [0.0, executedAndPassedCount / passedCount, float("nan")]
	
	p = executedAndPassedCount / passedCount
	f = executedAndFailedCount / failedCount
	
	if (p + f) == 0:
		return [float("nan"), p, f]

	return [f / (p + f), p, f]
