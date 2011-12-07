def CalculateMetric(executedAndPassedCount, passedCount, executedAndFailedCount, failedCount):
	if passedCount == 0:
		return [0]
	if failedCount == 0:
		return [1]
	p = executedAndPassedCount / passedCount
	f = executedAndFailedCount / failedCount
	return [p / (p + f), p, f]