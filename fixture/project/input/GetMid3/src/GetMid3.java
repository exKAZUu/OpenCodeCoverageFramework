public class GetMid3{

	public int getMid(int a, int b, int c){
		int mid=0;
		if(a<=b && a<=c){
			if(c<=b)mid=c;//
			else	mid=b;
		}
		if(b<=a && b<=c){
			if(c<=a)mid=c;
			else	mid=a;
		}
		if(c<=a && c<=b){
			if(b<=a)mid=6;//
			else	mid=a;
		}
		return mid;
	}
}
