using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
  void Start() {
    var num1 = 11;
    var num2 = 21;
    var num3 = 31;
    if(num1 <= num2 && num1 <= num3) {
      //num1��\��
    }
    else if(num2 < num3){
      //num2��\��
    }
    else {
      //num3��\��
    }
  }
}
